using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System.ComponentModel;

namespace CCPOS.Module.BusinessObjects.Address
{
    [DefaultProperty("FullAddress")]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Address : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Address(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        protected override void OnSaving()
        {
            fullAddress = ObjectFormatter.Format("{StreetAddress} {City} {Province} {BOCountry} {PostalCode}", this, EmptyEntriesMode.RemoveDelimiterWhenEntryIsEmpty);

            base.OnSaving();
        }

        [Persistent("FullAddress")]
        private string fullAddress;

        [PersistentAlias("fullAddress")]
        public string FullAddress
        {
            get
            {
                return fullAddress;
            }
        }

        private string _StreetAddress;
        public string StreetAddress
        {
            get { return _StreetAddress; }
            set { SetPropertyValue<string>(nameof(StreetAddress), ref _StreetAddress, value); }
        }

        private City _City;
        public City City
        {
            get { return _City; }
            set { SetPropertyValue<City>(nameof(City), ref _City, value); }
        }

        private BOCountry _BOCountry;
        [ModelDefault("Caption", "Country")]
        public BOCountry BOCountry
        {
            get { return _BOCountry; }
            set { SetPropertyValue<BOCountry>(nameof(BOCountry), ref _BOCountry, value); }
        }

        private Province _Province;
        public Province Province
        {
            get { return _Province; }
            set { SetPropertyValue<Province>(nameof(Province), ref _Province, value); }
        }

        private string _PostalCode;
        [ModelDefault("EditMaskType", "RegEx")]
        [ModelDefault("EditMask", @"[A-Z]\d[A-Z]-\d[A-Z]\d")]
        public string PostalCode
        {
            get { return _PostalCode; }
            set { SetPropertyValue<string>(nameof(PostalCode), ref _PostalCode, value); }
        }

        private string _ZipCode;
        [ModelDefault("EditMaskType", "RegEx")]
        [ModelDefault("EditMask", @"\d{5}|(\d{5}-\d{4})")]
        public string ZipCode
        {
            get { return _ZipCode; }
            set { SetPropertyValue<string>(nameof(ZipCode), ref _ZipCode, value); }
        }

        private State _State;
        public State State
        {
            get { return _State; }
            set { SetPropertyValue<State>(nameof(State), ref _State, value); }
        }

    }
}