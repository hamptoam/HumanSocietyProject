using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {
        static HumanSocietyDataContext db;
        private static Employee employee;

        static Query()
        {
            db = new HumanSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();

            return allStates;
        }

        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }

            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;

            // submit changes
            db.SubmitChanges();
        }

        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }


        //// TODO Items: ////

        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {

            Func<Employee, Employee> queryMethod;

            switch (crudOperation)
            {
                case "read":
                    GetEmployee();
                    break;
                case "create":
                    CreateEmployee();
                    break;
                case "update":
                    upDateEmployee();
                    break;


            }
        }




        


        public static void CreateEmployee()
        {
            Console.Clear();
            string email = UserInterface.GetStringData("email", "your");
            int employeeNumber = int.Parse(UserInterface.GetStringData("employee number", "your"));
            try
            {
                employee = Query.RetrieveEmployeeUser(email, employeeNumber);
            }
            catch
            {
                UserInterface.DisplayUserOptions("Employee not found please contact your administrator");
                PointOfEntry.Run();
            }
            if (employee.Password != null)
            {
                UserInterface.DisplayUserOptions("User already in use please log in or contact your administrator");
               
                return;
            }
            else
            {
                upDateEmployee();
            }
        }

        public static void GetEmployee()
        {
            Console.Clear();
            string username = UserInterface.GetStringData("username", "your");
            if (Query.CheckEmployeeUserNameExist(username))
            {
                UserInterface.DisplayUserOptions("Username already in use please try another username.");
                GetUserName();
            }
            else
            {
                employee.UserName = username;
                UserInterface.DisplayUserOptions("Username successful");
            }
        }
        public static void upDateEmployee()
        {
            GetUserName();
            GetPassword();
            Query.AddUsernameAndPassword(employee);
        }

        public static void GetPassword()
        {
            UserInterface.DisplayUserOptions("Please enter your password: (CaSe SeNsItIvE)");
            employee.Password = UserInterface.GetUserInput();
        }

        public static void GetUserName ()
        {
            Console.Clear();
            string username = UserInterface.GetStringData("username", "your");
            if (Query.CheckEmployeeUserNameExist(username))
            {
                UserInterface.DisplayUserOptions("Username already in use please try another username.");
                GetUserName();
            }
            else
            {
                employee.UserName = username;
                UserInterface.DisplayUserOptions("Username successful");
            }
        }


        // TODO: Animal CRUD Operations


        internal static void AddAnimal(Animal animal)
        {
            HumanSocietyDataContext MyTable = new HumanSocietyDataContext();
            MyTable.Animals.InsertOnSubmit(animal);
            MyTable.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id)
        {
            var db = new HumanSocietyDataContext();
            var animalResult = db.Animals.Where(a => a.AnimalId == id).FirstOrDefault();
            return animalResult;
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            var db = new HumanSocietyDataContext();
            var newInfo = db.Animals.Where(n => n.AnimalId == animalId);

                 foreach (var n in newInfo)

            {
                db.SubmitChanges();
            }
        }

        internal static void RemoveAnimal(Animal animal)
        {
            HumanSocietyDataContext MyTable = new HumanSocietyDataContext();

            var animalToDelete = MyTable.Animals.Where(a => a.AnimalId == AnimalId);
            MyTable.Animals.DeleteOnSubmit(animal);

        }

        // TODO: Animal Multi-Trait Search

        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            

            //dana doing this one 
        }

        // TODO: Misc Animal Things
        internal static Category GetCategoryId(int CategoryId)
        {
            var db = new HumanSocietyDataContext();
            var CategoryResult  = db.Categories.Where(a => a.CategoryId == CategoryId).FirstOrDefault();
            return CategoryResult;
        } //fixed

        internal static Room GetRoom(int animalId)
        {

            var db = new HumanSocietyDataContext();
            var RoomResult = db.Rooms.Where(a => a.AnimalId == animalId).FirstOrDefault();
            return RoomResult;

        }

        internal static int GetDietPlanId(int DietPlanId)
        {

            var db = new HumanSocietyDataContext();
            var animalResult = db.DietPlans.Where(a => a.DietPlanId == DietPlanId).FirstOrDefault();
            return DietPlanId;

        }

        // TODO: Adoption CRUD Operations

            
            public delegate void GetAnimalByIdPtr();
            public delegate void GetClientByIdPtr();
           internal static Adoption Adopt(Animal animal, Client client)
            {

                var adoption = new Adoption();
                var animal = new Animal();
                var client = new Client();


                Animal GetAnimalByIdPtr = new GetAnimalByIdPtr(GetAnimalById();
                object inkvoke();
            
            
                var db = new HumanSocietyDataContext();
                db.Adoptions.InsertOnSubmit(adoption);
                db.SubmitChanges();
                set { Adoption.ApprovalStatus == "Adopted" };
                
            }
            
        

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {

            var db = new HumanSocietyDataContext();
            return db.Adoptions.Where(c => c.ApprovalStatus == "pending");
        }

        internal static IQueryable<Adoption> GetApprovedAdoptions()
        {
            var db = new HumanSocietyDataContext();
            return db.Adoptions.Where(c => c.ApprovalStatus == "approved");

        }

        internal static IQueryable<Adoption> GetDeniedAdoptions()
        {
            var db = new HumanSocietyDataContext();
            return db.Adoptions.Where(c => c.ApprovalStatus == "denied");

        }

        

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {

         //   Adoption.ApprovalStatus == "denied"; ? 
            var db = new HumanSocietyDataContext();
            Adoption adoptionResult = db.Adoptions.Where(c => c.AdoptionFee == adoption.AnimalId).First();
           
            db.SubmitChanges();

        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            var db = new HumanSocietyDataContext();
         //   foreach (animalId in Animal)
            {



                






            }

        }


        /*
        //TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            var db = new HumanSocietyDataContext();


            results = db.Animals.Where(c => c.AnimalId == Animal.AnimalId).ToList();
            return results;

        }

        */
        internal static void UpdateShot(string shotName, Animal animal)
        {
            var db = new HumanSocietyDataContext();
            var shot = new Shot();

        }
        public static void AddNewShot(string name)
        {
            var db = new HumanSocietyDataContext();
            var shot = new Shot();
            shot.Name = name;

            db.Shots.InsertOnSubmit(shot);

            db.SubmitChanges();
        }
    }
}