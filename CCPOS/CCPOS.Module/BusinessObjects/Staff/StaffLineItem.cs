using System;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using CCPOS.Module.BusinessObjects.Inventory;
using DevExpress.ExpressApp.Security.Strategy;
using CCPOS.Module.BusinessObjects.Sales;
using CCPOS.Module.Utils;
using DevExpress.ExpressApp.ConditionalAppearance;

namespace CCPOS.Module.BusinessObjects.Staff
{
    [VisibleInReports(true)]
    //[DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class StaffLineItem : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public StaffLineItem(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            Employee = SecuritySystem.CurrentUser.ToString();
            DateCreated = DateTime.Now;
        }

        private string[] refreshViews = new string[] { "Product_ListView", "StaffLineItem_ListView" };

        protected override void OnSaving()
        {
            fullName = ObjectFormatter.Format("{Product}-{Type}-{Employee}", this, EmptyEntriesMode.RemoveDelimiterWhenEntryIsEmpty);

            base.OnSaving();
            //amount should already be set, but to keep continuity incase they manually adjusted amount, set it based on values.
            if (Volume != VolumeType.Bottle)
                SetQuantity(Volume, Product);
            //refresh list views, mainly for Product and StaffLineItem list views.
            //AppHelper.RefreshListViews(refreshViews);
        }

        protected override void OnDeleting()
        {
            base.OnDeleting();
            //should just need to add Amount back to Quantity on deleting i think...
            if (Product != null) Product.Quantity += Quantity;
        }

        /// <summary>
        /// Current Quantity is added back to product, newVolume is used to reduce Product quantity accordingly
        /// </summary>
        /// <param name="volume">New VolumeType value</param>
        /// <param name="product">Product to change</param>
        private void SetQuantity(VolumeType volume, Product product)
        {
            if (product != null)
            {
                product.Quantity += this.Quantity;

                //calculte new Quantity based on VolumeType
                switch (volume)
                {
                    case VolumeType.Remainder:
                        decimal remainder = CalculateProductRemainder(product);
                        product.Quantity -= remainder;
                        this.Quantity = remainder;
                        break;
                    //case VolumeType.Bottle:
                    //    product.Quantity -= this.Quantity;
                    //    this.Quantity = this.Quantity;
                    //    break;
                    case VolumeType.Glass:
                        product.Quantity -= 1m / 4;
                        this.Quantity = 1m / 4;
                        break;
                    case VolumeType.Sample:
                        product.Quantity -= 1m / 12;
                        this.Quantity = 1m / 12;
                        break;
                    default:
                        break;
                }
                //no longer using Helcim online Store no Product inventory sync required.
                //HelcimHelper.SetHelcimInventory(product);
            }
        }

        /// <summary>
        /// Returns Product remainder value.
        /// </summary>
        /// <param name="product">Product to change</param>
        private decimal CalculateProductRemainder(Product product)
        {
            return product.Quantity - Math.Truncate(product.Quantity);
        }
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

        private Product _Product;
        [Association]
        public Product Product
        {
            get { return _Product; }
            set {
                if (!IsLoading)
                {
                    SetQuantity(Volume, value);
                }
                SetPropertyValue<Product>(nameof(Product), ref _Product, value); 
            }
        }

        private StaffLineItemType _Type;
        public StaffLineItemType Type
        {
            get { return _Type; }
            set { SetPropertyValue<StaffLineItemType>(nameof(Type), ref _Type, value); }
        }

        private VolumeType _Volume;
        [ImmediatePostData]
        public VolumeType Volume
        {
            get { return _Volume; }
            set {
                SetPropertyValue<VolumeType>(nameof(Volume), ref _Volume, value);
                //cant call SetQuantity before the new Volume is set. Otherwise it messes up when entering Quantity Setter from the Quantity change while the old Volume is still set. Was causing additional increments of the Product Quantity.
                if (!IsLoading)
                {
                    if (value != VolumeType.Bottle)
                        SetQuantity(value, Product);
                }
            }
        }

        private decimal _Quantity;
        [ModelDefault("DisplayFormat", "{0:n}")]
        [ModelDefault("EditMask", "n")]
        //[ModelDefault("AllowEdit", "False")]
        //[Volume] = 'Bottle'
        [Appearance("AddressOneIsEmpty", Enabled = false, Criteria = "[Volume] = 'Remainder' OR [Volume] = 'Glass' OR [Volume] = 'Sample'", Context = "DetailView")]
        [ImmediatePostData]
        public decimal Quantity
        {
            get { return _Quantity; }
            set {

                if (!IsLoading)
                {
                    if (Volume == VolumeType.Bottle && Product != null)
                    {
                        //add back old value.
                        Product.Quantity += _Quantity;
                        //subtract new
                        Product.Quantity -= value;
                    }
                }

                SetPropertyValue<decimal>(nameof(Quantity), ref _Quantity, value); 
            }
        }

        private string _Employee;
        public string Employee
        {
            get { return _Employee; }
            set { SetPropertyValue<string>(nameof(Employee), ref _Employee, value); }
        }

        private DateTime _DateCreated;
        public DateTime DateCreated
        {
            get { return _DateCreated; }
            set { SetPropertyValue<DateTime>(nameof(DateCreated), ref _DateCreated, value); }
        }

        private string _Notes;
        [Size(SizeAttribute.Unlimited)]
        public string Notes
        {
            get { return _Notes; }
            set { SetPropertyValue<string>(nameof(Notes), ref _Notes, value); }
        }
    }
}