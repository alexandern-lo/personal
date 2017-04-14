h1. Utils

h2. Key Value Coding (KVC)

Key Value Coding is a concept from iOS which is set of interfaces and techniques you use to access
any object as if it was a dictionary. Key difference with reflection is that when you access dictionary 
objects you get values stored in dictionary, not property values.

h3. KVC design

To work with object as if it is a dictionary you first lookup appropriate IKeyValueCoding implementation.
var kvc = KeyValueCoding.Impl<MyDTO>();

Behind the scene this code searches for IKeyValueCoding implementation for MyDTO class, then for MyDTO interfaces,
then for MyDTO parent and so on. To get key you use Get method

kvc.Get(myDTO, "myKey");

and to set you use Set

kvc.Set(myDTO, "myKey", myValue);

There are number of other methods, please refer to specific method documentation or write your own :)

New IKeyValueCoding implementations can be registered via KeyValueCoding.Register which takes
type as a key and implementation as a value. Implementations are stored in ConcurrentDictionary and 
it is save to look them up in multiple threads. Nothing is done to ensure implementation thread safety.

h3. Record structure

Record structure provide you IDictionary like interface to any object via KVC. Internally it holds an
object and corresponding KVC implementation. All methods from Record is redirected then to corresponding KVC method.
Record struct might be useful when you want to work with Dictionary, JSON and object of same structure as if they were
dictionaries.