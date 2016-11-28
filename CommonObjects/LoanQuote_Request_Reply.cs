using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonObjects
{
    using System.Xml.Serialization;


    /// <remarks/>
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class LoanQuoteRequest
    {

        /// <remarks/>
        public int SSN;

        /// <remarks/>
        public System.Double LoanAmount;

        /// <remarks/>
        public int LoanTerm;
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class LoanQuoteReply
    {

        /// <remarks/>
        public int SSN;

        /// <remarks/>
        public System.Double LoanAmount;

        /// <remarks/>
        public System.Double InterestRate;

        /// <remarks/>
        public string QuoteID;
    }
}
