namespace CommonObjects
{
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=true)]
    public class CreditBureauRequest {
        
        /// <remarks/>
        public int SSN;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=true)]
    public class CreditBureauReply {
        
        /// <remarks/>
        public int SSN;
        
        /// <remarks/>
        public int CreditScore;
        
        /// <remarks/>
        public int HistoryLength;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=true)]
    public class Run {
    }
}
