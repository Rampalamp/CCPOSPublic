using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace CCPOS.Module.BusinessObjects.Sales
{
    //[DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class DiscountType : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public DiscountType(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }


        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { SetPropertyValue<string>(nameof(Name), ref _Name, value); }
        }



        private int _FoodDiscount;
        [ModelDefault("DisplayFormat", "{0:N0}%")]
        [ModelDefault("EditMask", "N0")]
        public int FoodDiscount
        {
            get { return _FoodDiscount; }
            set { SetPropertyValue<int>(nameof(FoodDiscount), ref _FoodDiscount, value); }
        }

        private int _WineDiscount;
        [ModelDefault("DisplayFormat", "{0:N0}%")]
        [ModelDefault("EditMask", "N0")]
        public int WineDiscount
        {
            get { return _WineDiscount; }
            set { SetPropertyValue<int>(nameof(WineDiscount), ref _WineDiscount, value); }
        }


    }
}