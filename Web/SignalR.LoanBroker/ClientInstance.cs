namespace Web.SignalR.LoanBroker
{
    class ClientInstance
    {
        static SenderPropertyFields s_senderPropertyFields = new SenderPropertyFields();

        public static SenderPropertyFields PropertyFields
        {
            get { return s_senderPropertyFields; }
            set { s_senderPropertyFields = value; }
        }
    }
}
