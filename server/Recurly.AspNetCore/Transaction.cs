﻿using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Recurly.AspNetCore.Extensions;
using Recurly.AspNetCore.List;

namespace Recurly.AspNetCore
{
    public class Transaction : RecurlyEntity
    {
        // The currently valid Transaction States
        public enum TransactionState : short
        {
            All = 0,
            Unknown,
            Success,
            Failed,
            Voided,
            Declined
        }

        public enum TransactionType : short
        {
            All = 0,
            Unknown,
            Authorization,
            Purchase,
            Refund,
            Verify
        }

        public string Uuid { get; private set; }
        public TransactionType Action { get; set; }
        public int AmountInCents { get; set; }
        public int TaxInCents { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }

        public TransactionState Status { get; private set; }

        public string Reference { get; set; }

        public bool Test { get; private set; }
        public bool Voidable { get; private set; }
        public bool Refundable { get; private set; }

        public string IpAddress { get; private set; }

        public string CCVResult { get; private set; }
        public string AvsResult { get; private set; }
        public string AvsResultStreet { get; private set; }
        public string AvsResultPostal { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public string AccountCode { get; private set; }

        public Boolean TaxExempt { get; set; }
        public string TaxCode { get; set; }
        public string AccountingCode { get; set; }

        private Account account;
        public Account Account
        {
            get { return account ?? (account = Task.Run(async () => await Accounts.Get(AccountCode)).Result); }
            set
            {
                account = value;
                AccountCode = value.AccountCode;
            }
        }
        public int? Invoice { get; private set; }
        public string InvoicePrefix { get; private set; }

        public string InvoiceNumberWithPrefix()
        {
            return InvoicePrefix + Convert.ToString(Invoice);
        }

        internal Transaction()
        { }

        internal Transaction(XmlReader reader)
        {
            ReadXml(reader);
        }

        /// <summary>
        /// Creates a new transaction
        /// </summary>
        /// <param name="account"></param>
        /// <param name="amountInCents"></param>
        /// <param name="currency"></param>
        public Transaction(Account account, int amountInCents, string currency)
        {
            Account = account;
            AmountInCents = amountInCents;
            Currency = currency;
        }

        /// <summary>
        /// Creates a new transaction
        /// </summary>
        /// <param name="accountCode"></param>
        /// <param name="amountInCents"></param>
        /// <param name="currency"></param>
        public Transaction(string accountCode, int amountInCents, string currency)
        {
            AccountCode = accountCode;
            AmountInCents = amountInCents;
            Currency = currency;
        }

        internal const string UrlPrefix = "/transactions/";

        /// <summary>
        /// Creates an invoice, charge, and optionally account
        /// </summary>
        public void Create()
        {
             Client.Instance.PerformRequest(Client.HttpRequestMethod.Post,
                UrlPrefix,
                WriteXml,
                ReadXml);
        }

        /// <summary>
        /// Refunds a transaction
        /// 
        /// </summary>
        /// <param name="refund">If present, the amount to refund. Otherwise it is a full refund.</param>
        public void Refund(int? refund = null)
        {
            Client.Instance.PerformRequest(Client.HttpRequestMethod.Delete,
                UrlPrefix + Uri.EscapeUriString(Uuid) + (refund.HasValue ? "?amount_in_cents=" + refund.Value : ""),
                ReadXml);
        }

        public async Task<Invoice> GetInvoice()
        {
            return await Invoices.Get(InvoiceNumberWithPrefix());
        }


        #region Read and Write XML documents

        internal override void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                // End of account element, get out of here
                if ((reader.Name == "transaction") &&
                    reader.NodeType == XmlNodeType.EndElement)
                    break;

                if (reader.NodeType != XmlNodeType.Element) continue;

                string href;
                int amount;
                switch (reader.Name)
                {
                    case "account":
                        href = reader.GetAttribute("href");
                        if (null != href)
                            AccountCode = Uri.UnescapeDataString(href.Substring(href.LastIndexOf("/") + 1));
                        break;

                    case "invoice":
                        href = reader.GetAttribute("href");
                        if (null != href)
                        {
                            string invoiceNumber = href.Substring(href.LastIndexOf("/") + 1);
                            MatchCollection matches = Regex.Matches(invoiceNumber, "([^\\d]{2})(\\d+)");

                            if (matches.Count == 1) 
                            {
                                InvoicePrefix = matches[0].Groups[1].Value;
                                Invoice = int.Parse(matches[0].Groups[2].Value);
                            } 
                            else
                            {
                                Invoice = int.Parse(invoiceNumber);
                            }
                        }
                        break;

                    case "uuid":
                        Uuid = reader.ReadElementContentAsString();
                        break;

                    case "action":
                        Action = reader.ReadElementContentAsString().ParseAsEnum<TransactionType>();
                        break;

                    case "amount_in_cents":
                        if (Int32.TryParse(reader.ReadElementContentAsString(), out amount))
                            AmountInCents = amount;
                        break;

                    case "tax_in_cents":
                        if (Int32.TryParse(reader.ReadElementContentAsString(), out amount))
                            TaxInCents = amount;
                        break;

                    case "currency":
                        Currency = reader.ReadElementContentAsString();
                        break;
                        
                    case "description":
                        Description = reader.ReadElementContentAsString();
                        break;

                    case "status":
                        var state = reader.ReadElementContentAsString();
                        Status = "void" == state ? TransactionState.Voided : state.ParseAsEnum<TransactionState>();
                        break;

                    case "reference":
                        Reference = reader.ReadElementContentAsString();
                        break;

                    case "test":
                        Test = reader.ReadElementContentAsBoolean();
                        break;

                    case "voidable":
                        Voidable = reader.ReadElementContentAsBoolean();
                        break;

                    case "refundable":
                        Refundable = reader.ReadElementContentAsBoolean();
                        break;

                    case "ip_address":
                        IpAddress = reader.ReadElementContentAsString();
                        break;

                    case "ccv_result":
                        CCVResult = reader.ReadElementContentAsString();
                        break;

                    case "avs_result":
                        AvsResult = reader.ReadElementContentAsString();
                        break;

                    case "avs_result_street":
                        AvsResultStreet = reader.ReadElementContentAsString();
                        break;

                    case "avs_result_postal":
                        AvsResultPostal = reader.ReadElementContentAsString();
                        break;

                    case "created_at":
                        CreatedAt = reader.ReadElementContentAsDateTime();
                        break;

                    case "updated_at":
                        UpdatedAt = reader.ReadElementContentAsDateTime();
                        break;

                    case "details":
                        // API docs say not to load details into objects
                        break;
                }
            }
        }

        internal override void WriteXml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("transaction");

            xmlWriter.WriteElementString("amount_in_cents", AmountInCents.AsString());
            xmlWriter.WriteElementString("currency", Currency);
            xmlWriter.WriteStringIfValid("description", Description);

            xmlWriter.WriteElementString("tax_exempt", TaxExempt.AsString().ToLower());
            xmlWriter.WriteStringIfValid("tax_code", TaxCode);
            xmlWriter.WriteStringIfValid("accounting_code", AccountingCode);   

            if (Account != null)
            {
                Account.WriteXml(xmlWriter);
            }

            xmlWriter.WriteEndElement(); 
        }

        #endregion

        #region Object Overrides

        public override string ToString()
        {
            return "Recurly Transaction: " + Uuid;
        }

        public override bool Equals(object obj)
        {
            var transaction = obj as Transaction;
            return transaction != null && Equals(transaction);
        }

        public bool Equals(Transaction transaction)
        {
            return Uuid == transaction.Uuid;
        }

        public override int GetHashCode()
        {
            return Uuid.GetHashCode();
        }

        #endregion
    }

    public sealed class Transactions
    {
        private static readonly QueryStringBuilder Build = new QueryStringBuilder();
        /// <summary>
        /// Lists transactions by state and type. Defaults to all.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static RecurlyList<Transaction> List(TransactionList.TransactionState state = TransactionList.TransactionState.All,
            TransactionList.TransactionType type = TransactionList.TransactionType.All)
        {
            return new TransactionList("/transactions/" +
                Build.QueryStringWith(state != TransactionList.TransactionState.All ? "state=" +state.ToString().EnumNameToTransportCase() : "")
                .AndWith(type != TransactionList.TransactionType.All ? "type=" +type.ToString().EnumNameToTransportCase() : "")
            );
        }

        public static async Task<Transaction> Get(string transactionId)
        {
            var transaction = new Transaction();

            var statusCode = await Client.Instance.PerformRequest(Client.HttpRequestMethod.Get,
                Transaction.UrlPrefix + Uri.EscapeUriString(transactionId),
                transaction.ReadXml);

            return statusCode == HttpStatusCode.NotFound ? null : transaction;
        }
    }
}
