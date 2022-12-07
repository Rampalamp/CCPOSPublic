using CCPOS.Module.BusinessObjects.Address;
using CCPOS.Module.BusinessObjects.Inventory.Wine;
using CCPOS.Module.BusinessObjects.Sales;
using CCPOS.Module.BusinessObjects.Staff;
using CCPOS.Module.Utils;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using System;
using System.ComponentModel;

namespace CCPOS.Module.BusinessObjects.Inventory
{
    //[DefaultClassOptions]
    [DefaultProperty("Name")]
    [VisibleInReports(true)]
    //[OptimisticLocking(false)]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Product : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Product(Session session)
            : base(session)
        {
            //session.TrackPropertiesModifications = true;
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).

            //set DateFirstandLast stocked to curr datetime
            this.DateFirstStocked = DateTime.Now;
            this.DateLastStocked = DateTime.Now;

        }

        protected override void OnSaving()
        {
            fullName = ObjectFormatter.Format("{Name} {ProductNumber.Number} {Barcode} {PricePerUnit} {Year} {ProductType} {Producer} {Grape} {Description}", this, EmptyEntriesMode.RemoveDelimiterWhenEntryIsEmpty);

            if(ProductType != null)
                if (Quantity <= 0 && ProductType.Name.Contains("Wine")) 
                    XtraMessageBox.Show(Name + @" Remaining Quantity: " + Quantity.ToString("0.##"));

            if (ProductNumber == null)
                ProductNumber = new ProductNumber(Session);

            //calculate margins
            MarginAmount = PricePerUnit - CostPerUnit;
            //cant divide by 0.
            if (PricePerUnit != 0)
                Margin = MarginAmount / PricePerUnit;
            
            base.OnSaving();
        }

        //Below event gets called any time a field is edited, doing Session.CommitTransaction successfully commits and saves new data. but when closing the file if you havnt hit Save button it still asks if you wanna save, even though its all saved anyway. Not sure if I want to push this auto save function. This works pretty seemless on a smaller app like this, would probably turn into a nightmare for any large scale apps with hundreds of VCs or save alterations.

        //protected override void AfterChangeByXPPropertyDescriptor()
        //{
        //    base.AfterChangeByXPPropertyDescriptor();

        //    Session.CommitTransaction();
        //}

        #region Generic
        //NEED SIZE UNLIMITED ABOVE PERSISNT FULL NAME TO HAVE THE DATA TYPE IN SQL CORRECT......... LULOLULZ
        [Size(SizeAttribute.Unlimited)]
        [Persistent("FullName")]
        private string fullName;
        [PersistentAlias("fullName")]
        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string FullName
        {
            get
            {
                return fullName;
            }
        }

        private ProductNumber _ProductNumber;
        [DevExpress.Xpo.Aggregated]
        public ProductNumber ProductNumber
        {
            get { return _ProductNumber; }
            set { SetPropertyValue<ProductNumber>(nameof(ProductNumber), ref _ProductNumber, value); }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { SetPropertyValue<string>(nameof(Name), ref _Name, value); }
        }

        private string _Barcode;
        public string Barcode
        {
            get { return _Barcode; }
            set { SetPropertyValue<string>(nameof(Barcode), ref _Barcode, value); }
        }

        private string _Description;
        [Size(SizeAttribute.Unlimited)]
        public string Description
        {
            get { return _Description; }
            set { SetPropertyValue<string>(nameof(Description), ref _Description, value); }
        }

        private decimal _CostPerUnit;
        [ModelDefault("EditMask", "C")]
        public decimal CostPerUnit
        {
            get { return _CostPerUnit; }
            set { SetPropertyValue<decimal>(nameof(CostPerUnit), ref _CostPerUnit, value); }
        }

        private decimal _PricePerUnit;
        [ModelDefault("EditMask", "C")]
        public decimal PricePerUnit
        {
            get { return _PricePerUnit; }
            set { SetPropertyValue<decimal>(nameof(PricePerUnit), ref _PricePerUnit, value); }
        }

        private decimal _TakeOutPricePerUnit;
        [ModelDefault("EditMask", "C")]
        public decimal TakeOutPricePerUnit
        {
            get { return _TakeOutPricePerUnit; }
            set { SetPropertyValue<decimal>(nameof(TakeOutPricePerUnit), ref _TakeOutPricePerUnit, value); }
        }

        private decimal _MarginAmount;
        [ModelDefault("Caption", "Margin ($)")]
        [ModelDefault("EditMask", "C")]
        [ModelDefault("AllowEdit","False")]
        public decimal MarginAmount
        {
            get { return _MarginAmount; }
            set { SetPropertyValue<decimal>(nameof(MarginAmount), ref _MarginAmount, value); }
        }

        private decimal _Margin;
        [ModelDefault("Caption", "Margin (%)")]
        [ModelDefault("DisplayFormat", "{0:P}")]
        [ModelDefault("EditMask", "P")]
        [ModelDefault("AllowEdit", "False")]
        public decimal Margin
        {
            get { return _Margin; }
            set { SetPropertyValue<decimal>(nameof(Margin), ref _Margin, value); }
        }



        private decimal _Quantity;
        [ModelDefault("DisplayFormat", "{0:n}")]
        [ModelDefault("EditMask", "n")]
        public decimal Quantity
        {
            get { return _Quantity; }
            set
            {

                if (_Quantity < value)
                {
                    //new quantity value increased, therefore either a return or restock of inventory. set DateLastStocked to curdate
                    DateLastStocked = DateTime.Now;
                }

                SetPropertyValue<decimal>(nameof(Quantity), ref _Quantity, value);
            }
        }


        private int _HelcimQuantity;
        [Browsable(false)]
        public int HelcimQuantity
        {
            get { return _HelcimQuantity; }
            set { SetPropertyValue<int>(nameof(HelcimQuantity), ref _HelcimQuantity, value); }
        }

        private DateTime _DateFirstStocked;
        [ModelDefault("Caption", "First Stocked")]
        public DateTime DateFirstStocked
        {
            get { return _DateFirstStocked; }
            set { SetPropertyValue<DateTime>(nameof(DateFirstStocked), ref _DateFirstStocked, value); }
        }

        private DateTime _DateLastStocked;
        [ModelDefault("Caption", "Last Stocked")]
        public DateTime DateLastStocked
        {
            get { return _DateLastStocked; }
            set { SetPropertyValue<DateTime>(nameof(DateLastStocked), ref _DateLastStocked, value); }
        }

        private ProductType _ProductType;
        public ProductType ProductType
        {
            get { return _ProductType; }
            set { SetPropertyValue<ProductType>(nameof(ProductType), ref _ProductType, value); }
        }

        private Producer _Producer;
        public Producer Producer
        {
            get { return _Producer; }
            set { SetPropertyValue<Producer>(nameof(Producer), ref _Producer, value); }
        }

        [Association]
        public XPCollection<SaleLineItem> SaleLineItems
        {
            get { return GetCollection<SaleLineItem>(nameof(SaleLineItems)); }
        }

        [Association]
        public XPCollection<StaffLineItem> StaffLineItems
        {
            get { return GetCollection<StaffLineItem>(nameof(StaffLineItems)); }
        }
        #endregion

        #region WineBarSpecific
        private int _Year;
        [ModelDefault("Caption", "Vintage")]
        [ModelDefault("DisplayFormat", "{0:f0}")]
        [ModelDefault("EditMask", "f0")]
        public int Year
        {
            get { return _Year; }
            set { SetPropertyValue<int>(nameof(Year), ref _Year, value); }
        }

        private BOCountry _BOCountry;
        [ModelDefault("Caption", "Country")]
        public BOCountry BOCountry
        {
            get { return _BOCountry; }
            set { SetPropertyValue<BOCountry>(nameof(BOCountry), ref _BOCountry, value); }
        }

        private Grape _Grape;
        public Grape Grape
        {
            get { return _Grape; }
            set { SetPropertyValue<Grape>(nameof(Grape), ref _Grape, value); }
        }

        private decimal _PricePerGlass;
        [ModelDefault("EditMask", "C")]
        public decimal PricePerGlass
        {
            get { return _PricePerGlass; }
            set { SetPropertyValue<decimal>(nameof(PricePerGlass), ref _PricePerGlass, value); }
        }

        private decimal _PricePerSample;
        [ModelDefault("EditMask", "C")]
        public decimal PricePerSample
        {
            get { return _PricePerSample; }
            set { SetPropertyValue<decimal>(nameof(PricePerSample), ref _PricePerSample, value); }
        }


        private string _HelcimProductId;
        [ModelDefault("AllowEdit","False")]
        public string HelcimProductId
        {
            get { return _HelcimProductId; }
            set { SetPropertyValue<string>(nameof(HelcimProductId), ref _HelcimProductId, value); }
        }

        #endregion
    }


}