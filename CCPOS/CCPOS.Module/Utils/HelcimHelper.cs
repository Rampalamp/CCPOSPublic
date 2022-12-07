using CCPOS.Module.BusinessObjects.Address;
using CCPOS.Module.BusinessObjects.Customer;
using CCPOS.Module.BusinessObjects.Inventory;
using CCPOS.Module.BusinessObjects.Sales;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using DevExpress.XtraEditors;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Linq;
using CCPOS.Module.BusinessObjects.Sales.Payment;

namespace CCPOS.Module.Utils
{
    public class HelcimHelper
    {
        public static RestClient Client { get; set; }

        /// <summary>
        /// Start Helcim Sync Action Asynchronously 
        /// </summary>
        /// <param name="showMessage">Display sync commence and complete message./param>
        /// <param name="action">Which action to perform/param>
        public static void StartHelcimSync(bool showMessage, string action)
        {
            BackgroundWorker main = new BackgroundWorker();
            main.DoWork += Main_DoWork;
            main.RunWorkerCompleted += Main_RunWorkerCompleted;
            main.RunWorkerAsync(new HelcimWorkerClass(showMessage, action));
        }

        /// <summary>
        /// Start Helcim Sync Action Asynchronously Using A IList<Products> Collection
        /// </summary>
        /// <param name="showMessage">Display Message on Complete</param>
        /// <param name="action">Which action to perform</param>
        /// <param name="products">IList of product objects</param>
        public static void StartHelcimSync(bool showMessage, string action, IList<Product> products)
        {
            BackgroundWorker main = new BackgroundWorker();
            main.DoWork += Main_DoWork;
            main.RunWorkerCompleted += Main_RunWorkerCompleted;
            main.RunWorkerAsync(new HelcimWorkerClass(showMessage, action, products));
        }

        /// <summary>
        /// Start Helcim Sync Action Asynchronously Using a Product and StockChange value
        /// </summary>
        /// <param name="showMessage">Display Message on Complete</param>
        /// <param name="action">Which action to perform</param>
        /// <param name="product">Product to adjust stock for</param>
        /// <param name="stockChange">Amount and direction to adjust in (-)</param>
        public static void StartHelcimSync(bool showMessage, string action, Product product, int stockChange)
        {
            BackgroundWorker main = new BackgroundWorker();
            main.DoWork += Main_DoWork;
            main.RunWorkerCompleted += Main_RunWorkerCompleted;
            main.RunWorkerAsync(new HelcimWorkerClass(showMessage, action, product, stockChange));
        }

        private static void Main_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((bool)e.Result) XtraMessageBox.Show("Helcim Update/Sync complete.");

            AppHelper.RefreshListViews();
        }

        private static void Main_DoWork(object sender, DoWorkEventArgs e)
        {
            //set showMessge argument as result to be used on run worker completed.
            e.Result = (e.Argument as HelcimWorkerClass).ShowCompleteMessage;

            switch ((e.Argument as HelcimWorkerClass).Action)
            {
                case "newOrders":

                    XmlDocument orderData = new XmlDocument();

                    orderData.LoadXml(GetListFromHelcim("order"));

                    XmlNodeList orders = orderData.SelectNodes("//order");

                    ProcessHelcimOrders(orders);

                    break;
                case "updateProducts":
                    //check if object list was passed in
                    if ((e.Argument as HelcimWorkerClass).Products != null)
                    {
                        using (IObjectSpace space = AppHelper.Application.CreateObjectSpace())
                        {
                            //only grab wine products I assume.
                            UpdateHelcimInventory((e.Argument as HelcimWorkerClass).Products, space);
                        }
                    }
                    else
                    {
                        //if no Products filled out on HelcimWorker safe to assume this is a single product stock adjustment
                        using (IObjectSpace space = AppHelper.Application.CreateObjectSpace())
                        {
                            //only grab wine products I assume.
                            UpdateHelcimInventory(
                                space.GetObjects<Product>(
                                    new FunctionOperator(
                                        FunctionOperatorType.Contains,
                                        new OperandProperty("ProductType.Name"),
                                        new OperandValue("Wine")
                                    )
                                ), space);
                        }
                    }
                    break;
                case "updateProductStock":
                    using (IObjectSpace space = AppHelper.Application.CreateObjectSpace())
                    {
                        //pass in product retrieved by current space.
                        UpdateHelcimInventory(space.GetObject<Product>((e.Argument as HelcimWorkerClass).Product), space, (e.Argument as HelcimWorkerClass).StockChange);
                    }
                    break;
                case "resetProductStock":
                    using (IObjectSpace space = AppHelper.Application.CreateObjectSpace())
                    {
                        ResetHelcimInventory((e.Argument as HelcimWorkerClass).Products, space);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Initiates an update of HelcimInventory using Product details and local HelcimQuantity ASYNC
        /// </summary>
        /// <param name="product">Product to check adjustment on</param>
        public static void SetHelcimInventory(Product product)
        {
            //I THINK THIS BELOW LOGIC SHOULD WORK FOR KEEPING HELCIM INVENTORY UP TO DATE WITH MINIMAL API CALLS.
            //ONLY CALL HELCIM IF ITS ON LIVE DATABASE
            if (!ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString.Contains("WineBarLive"))
                return;
            //calculate stock change, rounding down  always cause we dont care about reaminders
            int stockChange = Convert.ToInt32(Math.Floor(product.Quantity)) - product.HelcimQuantity;
            //if 0 send off updateProduct action
            if (stockChange != 0)
            {
                //set internal tracking
                product.HelcimQuantity += stockChange;
                //calculate change and send off to helcim async 
                StartHelcimSync(false, "updateProductStock", product, stockChange);
            }
        }

        /// <summary>
        /// Initiates an update of HelcimInventory using local product quantity, and helcim product item quantity.
        /// </summary>
        /// <param name="product">Product to adjust on.</param>
        /// <param name="currentHelcimStock">Current stock in helcim database</param>
        public static void SetHelcimInventory(Product product,IObjectSpace space, int currentHelcimStock)
        {
            //calculate stock change, rounding down  always cause we dont care about reaminders
            int stockChange = Convert.ToInt32(Math.Floor(product.Quantity)) - currentHelcimStock;
            //if 0 send off updateProduct action
            if (stockChange != 0)
            {
                //Starting a helcim sync makes new BG worker, causing threading issues on the listviews refresh when worker finishes
                //calculate change and send off to helcim async 
                //StartHelcimSync(false, "updateProductStock", product, stockChange);
                UpdateHelcimInventory(product, space, stockChange);
            }
        }
        /// <summary>
        /// Updates or Creates Helcim Products from a list of Products
        /// </summary>
        /// <param name="products">List of Products to Update or Create</param>
        /// <param name="space">ObjectSpace to use</param>
        public static void UpdateHelcimInventory(IList<Product> products, IObjectSpace space)
        {

            Dictionary<string, string> editFields = new Dictionary<string, string>
                {
                    {"action","productEdit"},
                    {"name",""},
                    {"productId",""},
                    {"description",""},
                    {"sku",""},
                    {"barcode",""},
                    {"availability","1"},
                    {"availabilityOnline","1"},
                    {"price",""},
                    {"salePrice",""},
                    {"taxExempt","0"},
                };

            Dictionary<string, string> updateFields = new Dictionary<string, string>
                {
                    {"action","inventoryUpdate"},
                    {"productId",""},
                    {"stockChange",""},
                    {"note","Last Synced - " + DateTime.Now.ToString()},
                };

            XmlDocument productData;

            foreach (Product product in products as IList<Product>)
            {

                editFields["name"] = product.Name;
                editFields["productId"] = product.HelcimProductId;
                editFields["description"] = product.Description;
                editFields["sku"] = DateTime.Now.Ticks.ToString();
                editFields["barcode"] = product.Barcode;
                editFields["price"] = product.CostPerUnit.ToString("0.##");
                editFields["salePrice"] = product.PricePerUnit.ToString("0.##");

                productData = new XmlDocument();
                productData.LoadXml(PostToHelcim(editFields));
                //should only be 1 at index 0
                XmlNode helcimProduct = productData.SelectNodes("//product").Item(0);
                string helcimProductId = helcimProduct.SelectSingleNode("id").InnerText;
                //set newly created procuctId from Helcim if blank
                if (String.IsNullOrEmpty(product.HelcimProductId))
                {
                    //retreive product from background worker space.
                    //this change is not being done on initial object, so need to use the local helcimProductId 
                    //otherwise it creates new products on the update call. Their API was unclear about this, i did not expect it to make 
                    //new products on an Update call.
                    Product spaceProduct = space.GetObject(product);

                    if (spaceProduct != null)
                    {
                        spaceProduct.HelcimProductId = helcimProductId;
                        //round down and reset HelcimQuantity to match whats about to set Online store value
                        spaceProduct.HelcimQuantity = Convert.ToInt32(Math.Floor(product.Quantity));
                    }
                    space.CommitChanges();
                }
                //Update product with current quantity of product.
                decimal curStock = Convert.ToDecimal(helcimProduct.SelectSingleNode("stock").InnerText);
                //Helcim is stupid and dont allow a set of stock value, only the intended change in value can be sent
                updateFields["stockChange"] = Convert.ToInt32(Math.Truncate(product.Quantity) - curStock).ToString();
                updateFields["productId"] = helcimProductId;
                //don't need to do anything with the response data I don't think.
                //Helcim product should be updated with most recent data/quantity.
                PostToHelcim(updateFields);
            }
        }
        /// <summary>
        /// Updates Helcim Product stock using parameter stockChange
        /// </summary>
        /// <param name="product">Product to Update or Create</param>
        /// <param name="space">ObjectSpace to use</param>
        /// <param name="stockChange">Change in stock</param>
        public static void UpdateHelcimInventory(Product product, IObjectSpace space, int stockChange)
        {
            Dictionary<string, string> updateFields = new Dictionary<string, string>
                {
                    {"action","inventoryUpdate"},
                    {"productId",""},
                    {"stockChange",""},
                    {"note","Last Stock Change - " + DateTime.Now.ToString()},
                };

            //HelcimProductId required to make stockChange call
            if (!String.IsNullOrEmpty(product.HelcimProductId))
            {
                updateFields["productId"] = product.HelcimProductId;
                updateFields["stockChange"] = stockChange.ToString();
                //shouldn't need to do anything with the response.
                PostToHelcim(updateFields);
            }

        }

        public static void ResetHelcimInventory(IList<Product> products, IObjectSpace space)
        {

            XmlDocument helcimProductList = new XmlDocument();
            helcimProductList.LoadXml(GetListFromHelcim("product"));
            XmlNodeList helcimProducts = helcimProductList.SelectNodes("//product");
            
            //loop through each product
            foreach (XmlNode product in helcimProducts)
            {
                //SKU SHOULD BE PRODUCT NUMBER
                int sku;
                
                bool goodInt = Int32.TryParse(product.SelectSingleNode("sku").InnerText, out sku);

                Product curProd = null;

                if (goodInt)
                    curProd = space.GetObject<Product>(
                        products.Where(x => x.ProductNumber?.Number == sku).FirstOrDefault()
                        );

                if (curProd != null)
                {
                    //found product in local database using Helcim SKU, which SHOULD be the ProductNumber
                    curProd.HelcimQuantity = Convert.ToInt32(curProd.Quantity);
                    //set ProductId, we NEED this to update quantity as sale line items or staff line items are created.
                    //this function must be called EACH TIME a new product is made in local system and respective Helcim syste product
                    if (String.IsNullOrEmpty(curProd.HelcimProductId))
                        curProd.HelcimProductId = product.SelectSingleNode("id").InnerText;
                    //calculate difference and send change off to helcim.
                    int helcimQty = Convert.ToInt32(product.SelectSingleNode("stock").InnerText);
                    
                    SetHelcimInventory(curProd, space, helcimQty);

                    space.CommitChanges();
                }
            }
        }

        public static string GetListFromHelcim(string dataType)
        {
            Dictionary<string, string> postFields = new Dictionary<string, string>();
            //determine dataType, and set action based on desired results
            switch (dataType)
            {
                case "product":
                    postFields.Add("action", "productSearch");
                    break;
                case "order":
                    postFields.Add("action", "orderSearch");
                    break;
                case "transaction":
                    postFields.Add("action", "transactionSearch");
                    break;
                case "customer":
                    postFields.Add("action", "customerSearch");
                    break;
                default:
                    break;
            }

            return PostToHelcim(postFields);
        }

        public static string GetItemFromHelcim(string dataType, string id)
        {
            Dictionary<string, string> postFields = new Dictionary<string, string>();
            //determine dataType, and set action based on desired results
            switch (dataType)
            {
                case "product":
                    postFields.Add("action", "productView");
                    postFields.Add("productId", "id");
                    break;
                case "order":
                    postFields.Add("action", "orderView");
                    postFields.Add("orderNumber", id);
                    break;
                case "transaction":
                    postFields.Add("action", "");
                    break;
                case "customer":
                    postFields.Add("action", "");
                    break;
                default:
                    break;
            }

            return PostToHelcim(postFields);
        }

        /// <summary>
        /// Sends off all post fields to Helcim Rest API, Writes to file in the event of an error response returned.
        /// </summary>
        /// <param name="postFields">Rest parameters (Keys), Rest values (Values)</param>
        /// <returns>Returns REST response Content</returns>
        public static string PostToHelcim(Dictionary<string, string> postFields)
        {
            var request = new RestRequest(Method.POST);

            //not entirely sure if the below cookie header is required for my purposes.
            //request.AddHeader("Cookie", "PHPSESSID=c4qkhd16m8bcf388u37ciqhj53");

            request.AlwaysMultipartFormData = true;

            request.AddParameter("apiToken", ConfigurationManager.AppSettings["HelcimApiToken"]);
            request.AddParameter("accountId", ConfigurationManager.AppSettings["HelcimAccountId"]);

            //loop through any custom params other then api token and accountId
            foreach (var item in postFields)
            {
                request.AddParameter(item.Key, item.Value);
            }

            IRestResponse response = Client?.Execute(request);
            //check for a response - list calls to helcim api don't return a <response> tag in xml, should result in blank string and not write.
            string responseNumber = GetStringBetween("<response>", "</response>", response.Content);

            if (response.StatusCode != System.Net.HttpStatusCode.OK || responseNumber == "0")
            {
                //if enterring here either a bad httpstatus code got returned, or helcim responded with 0 meaning an error happened
                //during execution on their side I think, possibly due to bad data passed in.
                //create new error txt in WineBar document folder and write response content to it.
                WriteToErrorFile(response.Content);
            }
            return response.Content;
        }

        private static void ProcessHelcimOrders(XmlNodeList orders)
        {

            using (IObjectSpace space = AppHelper.Application.CreateObjectSpace())
            {
                //below should be set up to loop through orders, can apply this to any list from helcim.
                //not sure what kind of response fields we can get, right now I don't see a product list with orders.
                Sale sale;
                XmlDocument orderData;
                XmlNode orderDetails;
                DateTime dateCreated;

                foreach (XmlNode order in orders)
                {
                    //grab dateCreated shorten this process up in large sets of data returned from orderSearch action.
                    //Helcim dates are calgary time, 2 hours behind, add 2 to the first grab
                    dateCreated = DateTime.Parse(order.SelectSingleNode("dateCreated").InnerText).AddHours(2);
                    //process only last week of orders.
                    if (dateCreated.Date < DateTime.Today.AddDays(-10)) continue;
                    //grab token to verify if sale already exists
                    string token = order.SelectSingleNode("token").InnerText;

                    sale = space.FindObject<Sale>(new BinaryOperator("HelcimToken", token));

                    if (sale == null)//sale not found, therefore can assume this order has not been processed
                    {
                        //grab orderNumber to get full order details
                        string orderNumber = order.SelectSingleNode("orderNumber").InnerText;
                        //make separate call to helcim to grab orderView
                        orderData = new XmlDocument();

                        orderData.LoadXml(GetItemFromHelcim("order", orderNumber));

                        orderDetails = orderData.SelectSingleNode("//order");

                        if (orderDetails.SelectNodes("//item").Count == 0)
                        {
                            //if item cout is 0, can assume this order was made from the helcim terminal in house
                            //try and find sale in local DB that is associated with this order
                            Sale foundSale = TryMatchSaleWithOrder(orderDetails, space, token, orderNumber, dateCreated);

                            if (foundSale != null)
                            {
                                //set Helcim data to found Sale.
                                foundSale.HelcimToken = token;
                                foundSale.HelcimOrderNumber = orderNumber;
                                foundSale.HelcimDateCreated = dateCreated;
                                foundSale.HelcimOrderURL = orderDetails.SelectSingleNode("url").InnerText;
                            }
                            else
                            {
                                //write out to a text file just for our records, hopefully above works well.
                                WriteToNoOrderMatchFile(orderDetails);
                            }
                        }
                        else
                        {
                            CreateDataFromHelcimOrder(orderDetails, space, token, orderNumber, dateCreated);
                        }

                        space.CommitChanges();
                    }
                }
            }
        }

        private static void CreateDataFromHelcimOrder(XmlNode order, IObjectSpace space, string token, string orderNumber, DateTime dateCreated)
        {
            //for order of commitChanges (not sure if it matters), starting with customer, then to Sale and Sale line items.
            #region Customer
            Customer customer = null;
            string customerCode = order.SelectSingleNode("//customer//customerCode").InnerText;
            if (!String.IsNullOrEmpty(customerCode))
            {
                //if in here customerCode exists, search customer table with customerCode 
                customer = space.FindObject<Customer>(new BinaryOperator("HelcimCustomerCode", customerCode));

                if (customer == null) customer = CreateHelcimCustomer(order, space, customer, customerCode);

            }
            #endregion
            //init sale
            Sale sale = space.CreateObject<Sale>();

            sale.HelcimToken = token;
            sale.HelcimOrderNumber = orderNumber;
            sale.HelcimOrderURL = order.SelectSingleNode("url").InnerText;
            sale.HelcimDateCreated = dateCreated;
            //flag as new online sale for alerts
            sale.NewOnlineSale = true;
            sale.OnlineSale = true;
            sale.CustomerComment = order.SelectSingleNode("comments").InnerText;
            //set online sale discount
            sale.DiscountType = space.FindObject<DiscountType>(new BinaryOperator("Name", "Online Discount"));
            #region SaleLineItems
            //Assuming we can use the item SKU as our product Oid, loop through items and grab/set products for SaleLineItems
            XmlNodeList orderItems = order.SelectNodes("//item");
            Product product;
            SaleLineItem saleLineItem;
            foreach (XmlNode item in orderItems)
            {
                int sku;
                bool goodSku = Int32.TryParse(item.SelectSingleNode("sku").InnerText, out sku);

                product = space.FindObject<Product>(new BinaryOperator("ProductNumber.Number", sku));

                if (product != null)//if product found create and add new salelineitem
                {
                    saleLineItem = space.CreateObject<SaleLineItem>();
                    //Reduce helcimQty by item Quantity to prevent Inventory calculations from triggering a stock change to Helcim when doing Product Quantity inventory changes.
                    decimal quantity = Convert.ToDecimal(item.SelectSingleNode("quantity").InnerText);
                    product.HelcimQuantity -= Convert.ToInt32(quantity);
                    //should only need to add product, OnSave() and Setters should cover the rest.
                    saleLineItem.Quantity = quantity;
                    saleLineItem.Product = product;
                    //bottles only for wine product types
                    if(product.ProductType != null)
                    {
                        if (product.ProductType.Name.Contains("Food"))
                            saleLineItem.Pricing = PricingType.Food;
                        else
                            saleLineItem.Pricing = PricingType.TakeOut;
                    }
                    //may need to set SaleLineItem.Sale field, not sure if the association makes itself when doing the .Add below
                    sale.SaleLineItems.Add(saleLineItem);
                }
            }
            #endregion
            //Might need to run a sale.CalculateSale.. Yes I do
            sale.CalculateSale();
            #region PaymentLineItems
            //Order has a status of DUE if not paid I assume. If status is not DUE may have to see what a paid order looks like...
            //Helcim Status' - DUE/PAID/SHIPPED/COMPLETED/REFUNDED/IN PROGRESS
            PaymentLineItem newPay = null;
            switch (order.SelectSingleNode("status").InnerText)
            {
                case "DUE":
                    break;
                case "PAID":
                    //make payment line item? Probably have to do yet another call to helcim to grab transaction details...
                    //should always be paid I think.
                    newPay = space.CreateObject<PaymentLineItem>();
                    newPay.Amount = sale.Total;
                    sale.PaymentLineItems.Add(newPay);
                    //everything else auto sets thats needed I think.
                    break;
                case "SHIPPED":
                    break;
                case "COMPLETED":
                    break;
                case "REFUNDED":
                    newPay = space.CreateObject<PaymentLineItem>();
                    newPay.Amount = sale.Total;
                    newPay.Type = PaymentType.Refund;
                    sale.PaymentLineItems.Add(newPay);
                    break;
                case "IN PROGRESS":
                    break;
                default:
                    break;
            }
            #endregion

            //final set of customer and sale if customer was created.
            if (customer != null) customer.Sales.Add(sale);
            //unsure if I needto set Customer on Sale if I add sale to the Sales list on Customer... Need to test this
            //sale.Customer = customer;

        }

        private static Sale TryMatchSaleWithOrder(XmlNode order, IObjectSpace space, string token, string orderNumber, DateTime dateCreated)
        {
            //** MIGHT BE ABLE TO USE CUSTOMER CODE AND CHECK IF THAT CUSTOMER EXISTS IN DB? NEED MORE HELCIM DATA
            //think we only realy have dateCreated and Total to work with. Likely need to first pull a list of Sales with the same Amount
            //then further specify to date the Receipt was generated on Sale, and dateCreated on Order.
            string total = order.SelectSingleNode("amount").InnerText;
            //pulls sales that match the total, and that has been created on that orders day with no HelcimOrderNumber associated with it.
            //only grab sales within last 2 hours of helcim order date created. kind of silly cause i increase it by 2 hours cause they work in calgary time, but i guess for future dateCreated uses it should be in est standard
            List<Sale> foundSales = space.GetObjects<Sale>(
                CriteriaOperator.And(
                    new BinaryOperator("Total", Convert.ToDecimal(total)),
                    new BinaryOperator("DateCreated", dateCreated.AddHours(-3), BinaryOperatorType.GreaterOrEqual),
                    new NullOperator("HelcimOrderNumber"),
                    new NotOperator(new NullOperator("Receipt"))
                    //new BinaryOperator("DateCreated", dateCreated.Date.AddDays(1).AddTicks(-1), BinaryOperatorType.GreaterOrEqual)
                    )
                ).ToList();

            //if (foundSales.Count == 1) return foundSales.FirstOrDefault();

            //if (foundSales.Count > 1)
            //{
            //    return GetOneSaleFromMinutesDescending(foundSales, 40, dateCreated);
            //}

            //Fairly certain its impossible to figure out for sure between multiple sales with same totals with printed receipts all within the last 3 hours of sales from when helcim order dateCreated... Just grab last i spose.
            //order by ascending, first date should be the earlier date I think... No winning here, thanks helcim!
            return foundSales.OrderBy(x => x.Receipt.DateCreated).FirstOrDefault();
        }

        /// <summary>
        /// Loops minutes one at a time up to minute max in an attempt to return a single Sale based on order DateCreated
        /// </summary>
        /// <param name="foundSales">List of Sales to search</param>
        /// <param name="minutes">Number of minutes to try and return one sale</param>
        /// <param name="dateCreated">Order DateCreated to Compare with.</param>
        /// <returns></returns>
        private static Sale GetOneSaleFromMinutesDescending(List<Sale> foundSales, int minutes, DateTime dateCreated)
        {
            List<Sale> minuteSales;
            for (int i = 0; i < minutes; i++)
            {
                minuteSales = foundSales.Where(x => x.Receipt?.DateCreated < dateCreated.AddMinutes(-i)).ToList();

                if (minuteSales.Count == 1) return minuteSales.FirstOrDefault();
            }

            return null;
        }

        private static Customer CreateHelcimCustomer(XmlNode order, IObjectSpace space, Customer customer, string customerCode)
        {
            //create customer, Most of customer information can be grabbed from the BillingAddress on the order
            customer = space.CreateObject<Customer>();

            customer.HelcimCustomerCode = customerCode;

            XmlNode billingAddress = order.SelectSingleNode("billingAddress");

            string[] splitName = billingAddress.SelectSingleNode("contactName").InnerText.Split(' ');

            if (splitName.Length == 2)
            {
                customer.FirstName = splitName[0];
                customer.LastName = splitName[1];
            }
            else
            {
                //not sure what to do with middle names, going to both first index into FirstName and everything else into LastName
                for (int i = 0; i < splitName.Length - 1; i++)
                {
                    if (i == 0)
                    {
                        customer.FirstName = splitName[i];
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(customer.LastName))
                        {
                            customer.LastName = splitName[i];
                        }
                        else
                        {
                            customer.LastName += " " + splitName[i];
                        }
                    }
                }

            }

            customer.Email = billingAddress.SelectSingleNode("email").InnerText;
            customer.PhoneNumber = billingAddress.SelectSingleNode("phone").InnerText;
            //customer address - should be initialized from the AfterConstruction
            customer.Address.StreetAddress = billingAddress.SelectSingleNode("street1").InnerText;
            customer.Address.PostalCode = billingAddress.SelectSingleNode("postalCode").InnerText;
            customer.Address.City = space.FindObject<City>(new BinaryOperator("Name", billingAddress.SelectSingleNode("city").InnerText));

            //skip if differs from deault Ontario/Canada
            if (billingAddress.SelectSingleNode("province").InnerText != "Ontario")
            {
                customer.Address.Province = space.FindObject<Province>(new BinaryOperator("Name", billingAddress.SelectSingleNode("province").InnerText));
            }
            if (billingAddress.SelectSingleNode("country").InnerText != "Canada")
            {
                customer.Address.Province = space.FindObject<Province>(new BinaryOperator("Name", billingAddress.SelectSingleNode("country").InnerText));
            }

            return customer;
        }
        /// <summary>
        /// Returns the string existing between from and to parameters of given text.
        /// </summary>
        /// <param name="from">Starting string</param>
        /// <param name="to">Ending string</param>
        /// <param name="text">Full text to search</param>
        /// <returns>String, or blank string if index of from or to is not found.</returns>
        private static string GetStringBetween(string from, string to, string text)
        {
            int posOne = text.IndexOf(from) + from.Length;
            int posTwo = text.IndexOf(to);
            //jump out if one of the strings not found
            if (posOne == -1 || posTwo == -1) return "";

            return text.Substring(posOne, posTwo - posOne);
        }

        private static void WriteToErrorFile(string content)
        {
            string localDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string savePath = localDocs + @"\TheWineBar\HelcimErrors\";

            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

            using (StreamWriter sw = new StreamWriter(savePath + String.Format(@"HelcimError{0}.txt", DateTime.Now.Ticks), false))
            {
                sw.Write(content);
            }

        }

        private static void WriteToNoOrderMatchFile(XmlNode order)
        {
            string localDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string savePath = localDocs + @"\TheWineBar\OrderMatchFailures\";

            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

            using (StreamWriter sw = new StreamWriter(savePath + String.Format(@"OrderMatchFailure-{0}.txt", order.SelectSingleNode("orderNumber").InnerText), true))
            {
                sw.WriteLine("**NO MATCH**");
                sw.WriteLine(order.SelectSingleNode("orderNumber").InnerText);
                sw.WriteLine(order.SelectSingleNode("url").InnerText);
                sw.WriteLine(DateTime.Now.ToString());
            }
        }

    }

    public class HelcimWorkerClass
    {
        public bool ShowCompleteMessage { get; set; }
        public string Action { get; set; }
        public IList<Product> Products { get; set; }
        public Product Product { get; set; }
        public int StockChange { get; set; }

        public HelcimWorkerClass(bool ShowCompleteMessage, string Action)
        {
            this.ShowCompleteMessage = ShowCompleteMessage;
            this.Action = Action;
        }
        public HelcimWorkerClass(bool ShowCompleteMessage, string Action, IList<Product> Products)
        {
            this.ShowCompleteMessage = ShowCompleteMessage;
            this.Action = Action;
            this.Products = Products;
        }
        public HelcimWorkerClass(bool ShowCompleteMessage, string Action, Product Product, int StockChange)
        {
            this.ShowCompleteMessage = ShowCompleteMessage;
            this.Action = Action;
            this.Product = Product;
            this.StockChange = StockChange;
        }
    }

}
