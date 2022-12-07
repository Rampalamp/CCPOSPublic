using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.ComponentModel;
using System.Linq;

namespace CCPOS.Module.BusinessObjects.Sales.Payment
{
    [DefaultProperty("Amount")]
    //[DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class PaymentLineItem : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PaymentLineItem(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).

            DateCreated = DateTime.Now;

            Type = PaymentType.Payment;
        }

        protected override void OnSaving()
        {

            fullName = ObjectFormatter.Format("{Amount} {Sale}", this, EmptyEntriesMode.RemoveDelimiterWhenEntryIsEmpty);

            base.OnSaving();

            //if type is refund make negative
            if (Type == PaymentType.Refund)
            {
                Amount = Amount * -1;
            }

            //trigger the Sale onSave for inline payment method creations.
            Sale?.Save();
        }

        protected override void OnDeleting()
        {
            base.OnDeleting();
            //recalculate sale and exclude the current payment line item deleting
            Sale?.CalculateSale(this);
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

        private decimal _Amount;
        [ModelDefault("EditMask", "C")]
        [ImmediatePostData]
        public decimal Amount
        {
            get { return _Amount; }
            set
            {

                if (!IsLoading)
                {
                    if (Sale != null)
                    {
                        //if value is 0 use as a prompt to set amount to Sale.Total.
                        if (value == 0)
                        {
                            value = Sale.Total;
                        }
                        //Do balance/change calculations on sale when Amount of PaymentLineItem changes.
                        //Exclude the current line item in the .Where and add value manually after.
                        Sale.AmountReceived = Sale.PaymentLineItems.Where(x => x.Type != PaymentType.Void && x != this).Sum(x => x.Amount);
                        Sale.AmountReceived += value;
                        Sale.Balance = Sale.Total - Sale.AmountReceived;
                        //check to see if Change is due
                        if (Sale.Balance < 0)
                        {
                            //If entering here Customer probably paid with cash, otherwise an over amount was charged to credit or debit
                            //round and set to string to account for rounding of pennys on Change value
                            string rawChange = Math.Round(Sale.Balance, 2, MidpointRounding.AwayFromZero).ToString();
                            //get last digit of decimal
                            char pennyValue = rawChange.Last();
                            //determine penny value
                            if (pennyValue == '3' || pennyValue == '4')
                            {
                                pennyValue = '5';
                            }
                            else if (pennyValue == '1' || pennyValue == '2')
                            {
                                pennyValue = '0';
                            }
                            rawChange = rawChange.Substring(0, rawChange.Length - 1) + pennyValue;
                            //set Change to Balance value and set Balance to 0
                            Sale.Change = Convert.ToDecimal(rawChange);
                            Sale.Balance = 0;
                        }
                        else
                        {
                            Sale.Change = 0;
                        }
                    }
                }

                SetPropertyValue<decimal>(nameof(Amount), ref _Amount, value);
            }
        }


        private PaymentMethod _PaymentMethod;
        [ModelDefault("Caption", "Method Of Payment")]
        [RuleRequiredField]
        [ImmediatePostData]
        public PaymentMethod PaymentMethod
        {
            get { return _PaymentMethod; }
            set
            {
                if (!IsLoading)
                {
                    if (Amount == 0 && Sale != null)
                    {
                        Amount = Sale.Total;
                    }
                }
                SetPropertyValue<PaymentMethod>(nameof(PaymentMethod), ref _PaymentMethod, value);
            }
        }

        private PaymentType _Type;
        [RuleRequiredField]
        public PaymentType Type
        {
            get { return _Type; }
            set { SetPropertyValue<PaymentType>(nameof(Type), ref _Type, value); }
        }

        private DateTime _DateCreated;
        public DateTime DateCreated
        {
            get { return _DateCreated; }
            set { SetPropertyValue<DateTime>(nameof(DateCreated), ref _DateCreated, value); }
        }

        private Sale _Sale;
        [Association]
        public Sale Sale
        {
            get { return _Sale; }
            set { SetPropertyValue<Sale>(nameof(Sale), ref _Sale, value); }
        }

    }
}