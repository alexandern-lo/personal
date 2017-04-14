using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using SL4N;
using System.Diagnostics;

namespace LiveOakApp.Models.Services
{
    public class CachableRequest<T> where T : new()
    {
        ILogger LOG { get; set; }

        public T Result { get; private set; }
        public bool DataIsLoadedToCache { get; private set; } // true even if loaded data is null
        object _resultLock = new object();

        string Key { get; set; }
        string EtagKey { get; set; }
        Func<string, CancellationToken?, Task<ApiResult<T>>> FetchFunc { get; set; }

        CacheStorage Cache { get; set; }

        CancellationTokenSource cancellationTokenSource;

        Action AfterLoadedFromNetwork { get; set; }

        public CachableRequest(CacheStorage CacheStorage, string key, Func<string, CancellationToken?, Task<ApiResult<T>>> fetchFunc, Action afterLoadedFromNetwork = null)
        {
            Key = key;
            EtagKey = string.Format("{0}_etag", Key);
            FetchFunc = fetchFunc;
            AfterLoadedFromNetwork = afterLoadedFromNetwork;

            Cache = CacheStorage;
            LOG = LoggerFactory.GetLogger(string.Format("{0}({1})", GetType().Name, Key));
        }

        public async Task ReplaceCache(T newResult)
        {
            await Cache.InsertObject(Key, newResult);
            lock (_resultLock)
            {
                Result = newResult;
                DataIsLoadedToCache = true;
            }
            await DropETag();
        }

        public void DropInMemoryCache()
        {
            lock (_resultLock)
            {
                Result = default(T);
                DataIsLoadedToCache = false;
            }
        }

        readonly SemaphoreSlim LoadFromNetworkSemaphore = new SemaphoreSlim(1, 1);

        #region Loading

        public Task<bool> LoadFromCache()
        {
            var watch = Stopwatch.StartNew();
            try
            {
                return DoLoadFromCache();
            }
            finally
            {
                var elapsed = watch.ElapsedMilliseconds;
                LOG.Debug("DoLoadFromCache finished in {0} ms", elapsed);
            }
        }

        async Task<bool> DoLoadFromCache()
        {
            if (DataIsLoadedToCache)
            {
                LOG.Debug("cache: already in memory");
                return false;
            }
            T cachedObject;
            try
            {
                cachedObject = await Cache.GetObject<T>(Key);
            }
            catch (KeyNotFoundException ex)
            {
                LOG.Debug("cache: not found {0}", ex.Message);
                DataIsLoadedToCache = true;
                return false;
            }
            bool resultWasUpdated = false;
            lock (_resultLock)
            {
                if (!DataIsLoadedToCache)
                {
                    Result = cachedObject;
                    DataIsLoadedToCache = true;
                    resultWasUpdated = true;
                }
            }
            if (!resultWasUpdated)
            {
                LOG.Debug("cache: ignoring, server was first");
                return false;
            }
            if (Equals(Result, default(T)))
            {
                LOG.Debug("cache: empty");
                return true;
            }
            var resultList = Result as ICollection;
            var resultListCount = resultList != null ? string.Format("count: {0}", resultList.Count) : "";
            LOG.Debug("cache: ok {0}", resultListCount);
            return true;
        }

        public async Task<bool> LoadFromNetwork(CancellationToken? cancellationToken)
        {
            bool firstIn = await LoadFromNetworkSemaphore.WaitAsync(0);
            if (firstIn)
            {
                LOG.Debug("awaiting new task");
                bool result;
                try
                {
                    var watch = Stopwatch.StartNew();
                    try
                    {
                        result = await DoLoadFromNetwork(cancellationToken);
                    }
                    finally
                    {
                        var elapsed = watch.ElapsedMilliseconds;
                        LOG.Debug("DoLoadFromNetwork finished in {0} ms", elapsed);
                    }
                }
                finally
                {
                    LoadFromNetworkSemaphore.Release();
                }
                return result;
            }
            else
            {
                LOG.Debug("awaiting existing task");
                if (cancellationToken != null)
                    await LoadFromNetworkSemaphore.WaitAsync((CancellationToken)cancellationToken);
                else
                    await LoadFromNetworkSemaphore.WaitAsync();
                LoadFromNetworkSemaphore.Release();
                return DataIsLoadedToCache;
            }
        }

        async Task<bool> DoLoadFromNetwork(CancellationToken? cancellationToken)
        {
            var originalToken = cancellationToken ?? CancellationToken.None;
            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(originalToken);
            await LoadETagIfNeeded();
            var networkObject = await FetchFunc(ETag, cancellationTokenSource.Token);
            switch (networkObject.Status)
            {
                case ApiResultStatus.Ok:
                    LOG.Debug("server: ok");
                    lock (_resultLock)
                    {
                        Result = networkObject.Content;
                        DataIsLoadedToCache = true;
                    }
                    await Cache.InsertObject(Key, Result);
                    lock (_eTagLock)
                    {
                        ETag = networkObject.ETag;
                    }
                    await SaveETag();
                    var resultList = Result as ICollection;
                    var resultListCount = resultList != null ? string.Format("count: {0}", resultList.Count) : "";
                    LOG.Debug("server: ok {0}", resultListCount);
                    if (AfterLoadedFromNetwork != null)
                    {
                        AfterLoadedFromNetwork();
                    }
                    return true;
                case ApiResultStatus.NotModified:
                    LOG.Debug("server: not modified");
                    return false;
            }
            LOG.Error("server: unhandled status: {0}", networkObject.Status);
            return false;
        }

        public void CancelLoading()
        {
            cancellationTokenSource?.Cancel();
        }

        public async Task DropData()
        {
            await DropETag();
            await Cache.DropObject(Key);
            DropInMemoryCache();
        }

        #endregion Loading

        #region ETag

        string ETag { get; set; }
        object _eTagLock = new object();

        async Task LoadETagIfNeeded()
        {
            try
            {
                var newEtag = await Cache.GetObject<string>(EtagKey);
                lock (_eTagLock)
                {
                    if (ETag != null) return;
                    ETag = newEtag;
                }
                if (newEtag == null)
                {
                    LOG.Debug("etag: empty");
                    return;
                }
                LOG.Debug("etag: loaded: '{0}'", ETag);
            }
            catch (KeyNotFoundException ex)
            {
                LOG.Debug("etag: not found {0}", ex.Message);
                return;
            }
        }

        async Task SaveETag()
        {
            await Cache.InsertObject(EtagKey, ETag);
        }

        async Task DropETag()
        {
            lock (_eTagLock)
            {
                ETag = null;
            }
            await Cache.DropObject(EtagKey);
        }

        #endregion ETag
    }
}
