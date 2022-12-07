using CCPOS.Module.BusinessObjects.Address;
using CCPOS.Module.BusinessObjects.Events;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CCPOS.Module.BusinessObjects.Customer
{
    [DefaultProperty("FullName")]
    [VisibleInReports(true)]
    //[DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Customer : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Customer(Session session)
            : base(session)
        {

        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            if (Address is null)
            {
                //should always be null at this point, but who knows...
                Address = new Address.Address(Session);
                Address.Province = Session.FindObject<Province>(new BinaryOperator("Name", "Ontario"));
                Address.BOCountry = Session.FindObject<BOCountry>(new BinaryOperator("Name", "Canada"));
            }
            DateCreated = DateTime.Now;
        }

        protected override void OnSaving()
        {
            fullName = ObjectFormatter.Format("{FirstName} {LastName}", this, EmptyEntriesMode.RemoveDelimiterWhenEntryIsEmpty);

            base.OnSaving();
        }

        [Persistent("FullName")]
        private string fullName;
        [PersistentAlias("fullName")]
        public string FullName
        {
            get
            {
                return fullName;
            }
        }

        private string _FirstName;
        public string FirstName
        {
            get { return _FirstName; }
            set { SetPropertyValue<string>(nameof(FirstName), ref _FirstName, value); }
        }

        private string _LastName;
        public string LastName
        {
            get { return _LastName; }
            set { SetPropertyValue<string>(nameof(LastName), ref _LastName, value); }
        }

        private string _HelcimCustomerCode;
        public string HelcimCustomerCode
        {
            get { return _HelcimCustomerCode; }
            set { SetPropertyValue<string>(nameof(HelcimCustomerCode), ref _HelcimCustomerCode, value); }
        }

        private Address.Address _Address;
        [ExpandObjectMembers(ExpandObjectMembers.InDetailView)]
        public Address.Address Address
        {
            get { return _Address; }
            set { SetPropertyValue<Address.Address>(nameof(Address), ref _Address, value); }
        }

        private string _Email;
        public string Email
        {
            get { return _Email; }
            set { SetPropertyValue<string>(nameof(Email), ref _Email, value); }
        }

        private string _PhoneNumber;
        [ModelDefault("EditMask", "000-000-0000")]
        public string PhoneNumber
        {
            get { return _PhoneNumber; }
            set { SetPropertyValue<string>(nameof(PhoneNumber), ref _PhoneNumber, value); }
        }

        private DateTime _DateCreated;
        public DateTime DateCreated
        {
            get { return _DateCreated; }
            set { SetPropertyValue<DateTime>(nameof(DateCreated), ref _DateCreated, value); }
        }

        private bool _InQueue;
        [ModelDefault("AllowEdit","False")]
        public bool InQueue
        {
            get { return _InQueue; }
            set { SetPropertyValue<bool>(nameof(InQueue), ref _InQueue, value); }
        }

        private DateTime _QueueStartTime;
        [ModelDefault("AllowEdit", "False")]
        public DateTime QueueStartTime
        {
            get { return _QueueStartTime; }
            set { SetPropertyValue<DateTime>(nameof(QueueStartTime), ref _QueueStartTime, value); }
        }

        private int _CurrentPartySize;
        public int CurrentPartySize
        {
            get { return _CurrentPartySize; }
            set { SetPropertyValue<int>(nameof(CurrentPartySize), ref _CurrentPartySize, value); }
        }

        [Association]
        public XPCollection<Sales.Sale> Sales
        {
            get { return GetCollection<Sales.Sale>(nameof(Sales)); }
        }

        [Association]
        public XPCollection<Reservation> Reservations
        {
            get { return GetCollection<Reservation>(nameof(Reservations)); }
        }

    }
}