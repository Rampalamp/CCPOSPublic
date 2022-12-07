using CCPOS.Module.BusinessObjects.Inventory;
using CCPOS.Module.Utils;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;

namespace CCPOS.Module.BusinessObjects.Sales
{
    //[DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class SaleLineItem : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public SaleLineItem(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).

            Quantity = 1;
            DateCreated = DateTime.Now;
        }

        protected override void OnSaving()
        {
            fullName = ObjectFormatter.Format("{Sale} {Product} {Total}", this, EmptyEntriesMode.RemoveDelimiterWhenEntryIsEmpty);

            base.OnSaving();

            //stamp sale price of the product at the time of committing if sale price is 0.
            //Sale price stamp should now be taken care of in the Setter of PricingType. I believe that makes the below if irrelevant
            //if (SalePrice == 0 && Product != null) SalePrice = Product.PricePerUnit;
            //check for DiscountType again, in theory this should already be set depending work flow.
            ApplySaleDiscountType(Product);
            //calculate sale line items - Will jumps into Sale.
            CalculateSaleLineItem(Discount, Quantity);

            #region InventoryCalculations
            //if (Product != null)
            //{
            //    Product.Quantity -= Quantity;
            //    Product.Save();
            //}
            #endregion

        }

        protected override void OnDeleting()
        {
            base.OnDeleting();

            //recalc inventory
            CalculateInventoryQuantity(Quantity, 0, Product);
            //separate method used to recalculate sale with out having to commit changes
            Sale?.CalculateSale(this);
        }

        private void ApplySaleDiscountType(Product product)
        {
            if (Sale != null && Sale?.DiscountType != null)
            {
                if (product != null && product?.ProductType != null)
                {
                    if (product.ProductType.Name.Contains("Food"))
                    {
                        Discount = Sale.DiscountType.FoodDiscount;
                    }
                    else if (product.ProductType.Name.Contains("Wine"))
                    {
                        Discount = Sale.DiscountType.WineDiscount;
                    }
                }
            }
        }

        private void CalculateSaleLineItem(int Discount, decimal Quantity)
        {
            //include discount if its not 0, assume its a percentage
            //this calculation is also done on the Quantity/Discount setter so line items update when changed
            if (Discount > 0)
            {
                //assume its a whole number ie 10% discount or 10.5% therefore /100
                DiscountAmount = SalePrice * Quantity * (Convert.ToDecimal(Discount) / 100m);
                Total = (SalePrice * Quantity) - DiscountAmount;
            }
            else
            {
                Total = SalePrice * Quantity;
            }

            //Recalculate Sale Totals
            Sale?.CalculateSale();
        }

        /// <summary>
        /// Gets the difference in quantities and adjust inventory values based on current PricingType
        /// </summary>
        /// <param name="oldValue">Original quantity</param>
        /// <param name="newValue">New quantity</param>
        /// <param name="product">Product to change. If null nothing is done</param>
        private void CalculateInventoryQuantity(decimal oldValue, decimal newValue, Product product)
        {
            if (product != null)
            {
                //inventory logic... Always get the difference in value
                //Quantity on the object will show as 1, but reduce to actual size reduced of bottle/product based on PricingType.
                switch (Pricing)
                {
                    case PricingType.Bottle:
                        product.Quantity -= newValue - oldValue;
                        break;
                    case PricingType.Glass:
                        product.Quantity -= (newValue - oldValue) / 4;
                        break;
                    case PricingType.Sample:
                        product.Quantity -= (newValue - oldValue) / 12;
                        break;
                    case PricingType.Food:
                        product.Quantity -= newValue - oldValue;
                        break;
                    case PricingType.TakeOut:
                        product.Quantity -= newValue - oldValue;
                        break;
                    default:
                        break;
                }
                //no longer using Helcim online Store no Product inventory sync required.
                //HelcimHelper.SetHelcimInventory(product);
                //product.Save();
            }
        }

        /// <summary>
        /// Adjusts inventory based on a PricingType change as opposed to a Quantity change.
        /// </summary>
        /// <param name="oldPricing">Original PricingType</param>
        /// <param name="newPricing">New PricingType</param>
        /// <param name="quantity">Current quantity</param>
        /// <param name="product">Product to change. If null nothing is done</param>
        private void CalculateInventoryQuantity(PricingType oldPricing, PricingType newPricing, decimal quantity, Product product)
        {
            if (product != null)
            {
                //switch old first adjust product inventory qty
                switch (oldPricing)
                {
                    case PricingType.Bottle:
                        product.Quantity += quantity;
                        break;
                    case PricingType.Glass:
                        product.Quantity += quantity / 4;
                        break;
                    case PricingType.Sample:
                        product.Quantity += quantity / 12;
                        break;
                    case PricingType.Food:
                        product.Quantity += quantity;
                        break;
                    case PricingType.TakeOut:
                        product.Quantity += quantity;
                        break;
                    default:
                        break;
                }

                //switch new value to adjust for new product inventory
                switch (newPricing)
                {
                    case PricingType.Bottle:
                        product.Quantity -= quantity;
                        break;
                    case PricingType.Glass:
                        product.Quantity -= quantity / 4;
                        break;
                    case PricingType.Sample:
                        product.Quantity -= quantity / 12;
                        break;
                    case PricingType.Food:
                        product.Quantity -= quantity;
                        break;
                    case PricingType.TakeOut:
                        product.Quantity -= quantity;
                        break;
                    default:
                        break;
                }
                //no longer using Helcim online Store no Product inventory sync required.
                //HelcimHelper.SetHelcimInventory(product);
            }
        }

        private void SetSalePrice(PricingType pricing, Product product)
        {
            if (product != null)
            {
                switch (pricing)
                {
                    case PricingType.Bottle:
                        SalePrice = product.PricePerUnit;
                        break;
                    case PricingType.Glass:
                        SalePrice = product.PricePerGlass;
                        break;
                    case PricingType.Sample:
                        SalePrice = product.PricePerSample;
                        break;
                    case PricingType.Food:
                        SalePrice = product.PricePerUnit;
                        break;
                    case PricingType.TakeOut:
                        SalePrice = product.TakeOutPricePerUnit;
                        break;
                    default:
                        break;
                }
            }
        }

        private void SetPricingType(Product product)
        {
            if(product.ProductType != null)
            {
                if (product.ProductType.Name.Contains("Food"))
                {
                    Pricing = PricingType.Food;
                }
                else
                {
                    Pricing = PricingType.Bottle;
                }
            }
        }

        //NEED SIZE UNLIMITED ABOVE PERSISNT FULL NAME TO HAVE THE DATA TYPE IN SQL CORRECT......... LULOLULZ
        [Size(SizeAttribute.Unlimited)]
        [Persistent("FullName")]
        private string fullName;
        [PersistentAlias("fullName")]
        [Browsable(false)]
        public string FullName
        {
            get
            {
                return fullName;
            }
        }

        private Sale _Sale;
        [Association]
        public Sale Sale
        {
            get { return _Sale; }
            set { SetPropertyValue<Sale>(nameof(Sale), ref _Sale, value); }
        }

        private Product _Product;
        [Association]
        [ImmediatePostData]
        public Product Product
        {
            get { return _Product; }
            set
            {
                if (!IsLoading)
                {

                    if (value != null)
                    {
                        //set PricingType (if food or not essentially)
                        SetPricingType(value);
                        //stamp products price per unit
                        SetSalePrice(Pricing, value);

                        if (value != _Product)
                        {
                            //change in product, subtract quantity from cur value product
                            CalculateInventoryQuantity(0, Quantity, value);
                            // add quantitie back to old product
                            CalculateInventoryQuantity(Quantity, 0, _Product);

                        }
                    }
                    else
                    {
                        //product remove from line item, add value back to old product
                        CalculateInventoryQuantity(Quantity, 0, _Product);
                    }
                    //attempt to apply discount type once product is set
                    ApplySaleDiscountType(value);
                    //calculate new amounts, CalculateSaleLineItem calls the Sale object and calculates sale totals
                    CalculateSaleLineItem(Discount, Quantity);
                    //Calculate Quantities. On initital product set, set oldValue to 0
                }
                SetPropertyValue<Product>(nameof(Product), ref _Product, value);
            }
        }

        private PricingType _Pricing;
        [ImmediatePostData]
        public PricingType Pricing
        {
            get { return _Pricing; }
            set
            {
                if (!IsLoading)
                {
                    //set sale price to appropriate pricing type
                    SetSalePrice(value, Product);

                    if (value != _Pricing)
                    {
                        //change in pricing type, need to reverse Quantity from old pricing type, and subtract new Pricing type amount.
                        CalculateInventoryQuantity(_Pricing, value, Quantity, Product);
                        //recalculate totals for line item
                        CalculateSaleLineItem(Discount, Quantity);
                    }

                }
                SetPropertyValue<PricingType>(nameof(Pricing), ref _Pricing, value);
            }
        }

        private decimal _Quantity;
        [ModelDefault("DisplayFormat", "{0:n}")]
        [ModelDefault("EditMask", "n")]
        [ImmediatePostData]
        public decimal Quantity
        {
            get { return _Quantity; }
            set
            {

                if (!IsLoading)
                {
                    //calculate new amounts, CalculateSaleLineItem calls the Sale object and calculates sale totals
                    CalculateSaleLineItem(Discount, value);
                    //calc inventory
                    CalculateInventoryQuantity(_Quantity, value, Product);
                }

                SetPropertyValue<decimal>(nameof(Quantity), ref _Quantity, value);
            }
        }

        private int _Discount;
        [ModelDefault("DisplayFormat", "{0:N0}%")]
        [ModelDefault("EditMask", "N0")]
        [ImmediatePostData]
        public int Discount
        {
            get { return _Discount; }
            set
            {
                //if discount amount changed, calculate sale.
                if (!IsLoading && _Discount != value)
                {
                    //calculate new amounts, CalculateSaleLineItem calls the Sale object and calculates sale totals
                    CalculateSaleLineItem(value, Quantity);
                }

                SetPropertyValue<int>(nameof(Discount), ref _Discount, value);
            }
        }

        private decimal _DiscountAmount;
        [ModelDefault("EditMask", "C")]
        [ModelDefault("Caption", "Discount($)")]
        [ModelDefault("AllowEdit","False")]
        public decimal DiscountAmount
        {
            get { return _DiscountAmount; }
            set { SetPropertyValue<decimal>(nameof(DiscountAmount), ref _DiscountAmount, value); }
        }


        private decimal _Total;
        [ModelDefault("EditMask", "C")]
        public decimal Total
        {
            get { return _Total; }
            set { SetPropertyValue<decimal>(nameof(Total), ref _Total, value); }
        }

        private decimal _SalePrice;
        [ModelDefault("EditMask", "C")]
        public decimal SalePrice
        {
            get { return _SalePrice; }
            set { SetPropertyValue<decimal>(nameof(SalePrice), ref _SalePrice, value); }
        }

        private DateTime _DateCreated;
        public DateTime DateCreated
        {
            get { return _DateCreated; }
            set { SetPropertyValue<DateTime>(nameof(DateCreated), ref _DateCreated, value); }
        }

    }

}