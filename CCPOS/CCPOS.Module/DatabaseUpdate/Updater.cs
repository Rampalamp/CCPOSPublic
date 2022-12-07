using CCPOS.Module.BusinessObjects.Address;
using CCPOS.Module.BusinessObjects.Inventory;
using CCPOS.Module.BusinessObjects.Sales;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using System;

namespace CCPOS.Module.DatabaseUpdate
{
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Updating.ModuleUpdater
    public class Updater : ModuleUpdater
    {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion)
        {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            //string name = "MyName";
            //DomainObject1 theObject = ObjectSpace.FindObject<DomainObject1>(CriteriaOperator.Parse("Name=?", name));
            //if(theObject == null) {
            //    theObject = ObjectSpace.CreateObject<DomainObject1>();
            //    theObject.Name = name;
            //}
            //PermissionPolicyUser sampleUser = ObjectSpace.FindObject<PermissionPolicyUser>(new BinaryOperator("UserName", "User"));
            //if(sampleUser == null) {
            //    sampleUser = ObjectSpace.CreateObject<PermissionPolicyUser>();
            //    sampleUser.UserName = "User";
            //    sampleUser.SetPassword("");
            //}
            //PermissionPolicyRole defaultRole = CreateDefaultRole();
            //sampleUser.Roles.Add(defaultRole);

            #region UserCreations
            PermissionPolicyUser userAdmin = ObjectSpace.FindObject<PermissionPolicyUser>(new BinaryOperator("UserName", "Admin"));
            if (userAdmin == null)
            {
                userAdmin = ObjectSpace.CreateObject<PermissionPolicyUser>();
                userAdmin.UserName = "Admin";
                // Set a password if the standard authentication type is used
                userAdmin.SetPassword("bowlers");
            }

            PermissionPolicyUser wbAdmin = ObjectSpace.FindObject<PermissionPolicyUser>(new BinaryOperator("UserName", "TheWineBar"));
            if (wbAdmin == null)
            {
                wbAdmin = ObjectSpace.CreateObject<PermissionPolicyUser>();
                wbAdmin.UserName = "TheWineBar";
                // Set a password if the standard authentication type is used
                wbAdmin.SetPassword("WineBar2020");
            }


            // If a role with the Administrators name doesn't exist in the database, create this role
            PermissionPolicyRole adminRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "Administrators"));
            if (adminRole == null)
            {
                adminRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                adminRole.Name = "Administrators";
            }
            adminRole.IsAdministrative = true;

            //add init user admin roles
            wbAdmin.Roles.Add(adminRole);
            userAdmin.Roles.Add(adminRole);
            #endregion

            #region BusinessObjectInitCreations
            //Preset DiscountTypes
            DiscountType online = ObjectSpace.FindObject<DiscountType>(new BinaryOperator("Name", "Online Discount"));
            if (online == null)
            {
                online = ObjectSpace.CreateObject<DiscountType>();
                online.Name = "Online Discount";
            }
            //product types for WineBar
            ProductType red = ObjectSpace.FindObject<ProductType>(new BinaryOperator("Name", "Wine - Red"));
            if (red == null)
            {
                red = ObjectSpace.CreateObject<ProductType>();
                red.Name = "Wine - Red";
            }

            ProductType white = ObjectSpace.FindObject<ProductType>(new BinaryOperator("Name", "Wine - White"));
            if (white == null)
            {
                white = ObjectSpace.CreateObject<ProductType>();
                white.Name = "Wine - White";
            }

            ProductType rose = ObjectSpace.FindObject<ProductType>(new BinaryOperator("Name", "Wine - Rosé"));
            if (rose == null)
            {
                rose = ObjectSpace.CreateObject<ProductType>();
                rose.Name = "Wine - Rosé";
            }

            ProductType sparkling = ObjectSpace.FindObject<ProductType>(new BinaryOperator("Name", "Wine - Sparkling"));
            if (sparkling == null)
            {
                sparkling = ObjectSpace.CreateObject<ProductType>();
                sparkling.Name = "Wine - Sparkling";
            }

            ProductType sf = ObjectSpace.FindObject<ProductType>(new BinaryOperator("Name", "Wine - Sweet or Fortified"));
            if (sf == null)
            {
                sf = ObjectSpace.CreateObject<ProductType>();
                sf.Name = "Wine - Sweet or Fortified";
            }

            ProductType merch = ObjectSpace.FindObject<ProductType>(new BinaryOperator("Name", "Merchandise"));
            if (merch == null)
            {
                merch = ObjectSpace.CreateObject<ProductType>();
                merch.Name = "Merchandise";
            }

            ProductType food = ObjectSpace.FindObject<ProductType>(new BinaryOperator("Name", "Food"));
            if (food == null)
            {
                food = ObjectSpace.CreateObject<ProductType>();
                food.Name = "Food";
            }

            ProductType eve = ObjectSpace.FindObject<ProductType>(new BinaryOperator("Name", "Event"));
            if (eve == null)
            {
                eve = ObjectSpace.CreateObject<ProductType>();
                eve.Name = "Event";
            }

            //provinces
            Province ontario = ObjectSpace.FindObject<Province>(new BinaryOperator("Name", "Ontario"));
            if (ontario == null)
            {
                ontario = ObjectSpace.CreateObject<Province>();
                ontario.Name = "Ontario";
            }

            Province alberta = ObjectSpace.FindObject<Province>(new BinaryOperator("Name", "Alberta"));
            if (alberta == null)
            {
                alberta = ObjectSpace.CreateObject<Province>();
                alberta.Name = "Alberta";
            }

            Province bc = ObjectSpace.FindObject<Province>(new BinaryOperator("Name", "British Columbia"));
            if (bc == null)
            {
                bc = ObjectSpace.CreateObject<Province>();
                bc.Name = "British Columbia";
            }

            Province man = ObjectSpace.FindObject<Province>(new BinaryOperator("Name", "Manitoba"));
            if (man == null)
            {
                man = ObjectSpace.CreateObject<Province>();
                man.Name = "Manitoba";
            }

            Province nb = ObjectSpace.FindObject<Province>(new BinaryOperator("Name", "New Brunswick"));
            if (nb == null)
            {
                nb = ObjectSpace.CreateObject<Province>();
                nb.Name = "New Brunswick";
            }

            Province nfl = ObjectSpace.FindObject<Province>(new BinaryOperator("Name", "Newfoundland and Labrador"));
            if (nfl == null)
            {
                nfl = ObjectSpace.CreateObject<Province>();
                nfl.Name = "Newfoundland and Labrador";
            }

            Province ns = ObjectSpace.FindObject<Province>(new BinaryOperator("Name", "Nova Scotia"));
            if (ns == null)
            {
                ns = ObjectSpace.CreateObject<Province>();
                ns.Name = "Nova Scotia";
            }

            Province pei = ObjectSpace.FindObject<Province>(new BinaryOperator("Name", "Prince Edward Island"));
            if (pei == null)
            {
                pei = ObjectSpace.CreateObject<Province>();
                pei.Name = "Prince Edward Island";
            }

            Province qb = ObjectSpace.FindObject<Province>(new BinaryOperator("Name", "Quebec"));
            if (qb == null)
            {
                qb = ObjectSpace.CreateObject<Province>();
                qb.Name = "Quebec";
            }

            Province sas = ObjectSpace.FindObject<Province>(new BinaryOperator("Name", "Saskatchewan"));
            if (sas == null)
            {
                sas = ObjectSpace.CreateObject<Province>();
                sas.Name = "Saskatchewan";
            }

            //countries
            BOCountry can = ObjectSpace.FindObject<BOCountry>(new BinaryOperator("Name", "Canada"));
            if (can == null)
            {
                can = ObjectSpace.CreateObject<BOCountry>();
                can.Name = "Canada";
            }

            BOCountry aus = ObjectSpace.FindObject<BOCountry>(new BinaryOperator("Name", "Australia"));
            if (aus == null)
            {
                aus = ObjectSpace.CreateObject<BOCountry>();
                aus.Name = "Australia";
            }

            BOCountry nz = ObjectSpace.FindObject<BOCountry>(new BinaryOperator("Name", "New Zealand"));
            if (nz == null)
            {
                nz = ObjectSpace.CreateObject<BOCountry>();
                nz.Name = "New Zealand";
            }

            BOCountry fr = ObjectSpace.FindObject<BOCountry>(new BinaryOperator("Name", "France"));
            if (fr == null)
            {
                fr = ObjectSpace.CreateObject<BOCountry>();
                fr.Name = "France";
            }

            BOCountry italy = ObjectSpace.FindObject<BOCountry>(new BinaryOperator("Name", "Italy"));
            if (italy == null)
            {
                italy = ObjectSpace.CreateObject<BOCountry>();
                italy.Name = "Italy";
            }

            BOCountry usa = ObjectSpace.FindObject<BOCountry>(new BinaryOperator("Name", "USA"));
            if (usa == null)
            {
                usa = ObjectSpace.CreateObject<BOCountry>();
                usa.Name = "USA";
            }
            //city inits
            City oaks = ObjectSpace.FindObject<City>(new BinaryOperator("Name", "Oakville"));
            if (oaks == null)
            {
                oaks = ObjectSpace.CreateObject<City>();
                oaks.Name = "Oakville";
            }

            City tor = ObjectSpace.FindObject<City>(new BinaryOperator("Name", "Toronto"));
            if (tor == null)
            {
                tor = ObjectSpace.CreateObject<City>();
                tor.Name = "Toronto";
            }

            City miss = ObjectSpace.FindObject<City>(new BinaryOperator("Name", "Mississauga"));
            if (miss == null)
            {
                miss = ObjectSpace.CreateObject<City>();
                miss.Name = "Mississauga";
            }

            City cats = ObjectSpace.FindObject<City>(new BinaryOperator("Name", "St. Catharines"));
            if (cats == null)
            {
                cats = ObjectSpace.CreateObject<City>();
                cats.Name = "St. Catharines";
            }

            City bur = ObjectSpace.FindObject<City>(new BinaryOperator("Name", "Burlington"));
            if (bur == null)
            {
                bur = ObjectSpace.CreateObject<City>();
                bur.Name = "Burlington";
            }

            #endregion

            ObjectSpace.CommitChanges(); //This line persists created object(s).
        }
        public override void UpdateDatabaseBeforeUpdateSchema()
        {
            base.UpdateDatabaseBeforeUpdateSchema();
            //if(CurrentDBVersion < new Version("1.1.0.0") && CurrentDBVersion > new Version("0.0.0.0")) {
            //    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName");
            //}
        }
        private PermissionPolicyRole CreateDefaultRole()
        {
            PermissionPolicyRole defaultRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "Default"));
            if (defaultRole == null)
            {
                defaultRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                defaultRole.Name = "Default";

                defaultRole.AddObjectPermission<PermissionPolicyUser>(SecurityOperations.Read, "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                defaultRole.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Allow);
                defaultRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                defaultRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "StoredPassword", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Deny);
                defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
            }
            return defaultRole;
        }
    }
}
