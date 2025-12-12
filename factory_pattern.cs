using System;

// Interface defining common CRUD operations
public interface IDataAccess
{
    void Create(string data);
    string Read(int id);
    void Update(int id, string data);
    void Delete(int id);
}

// LINQ implementation
public class LinqDataAccess : IDataAccess
{
    public void Create(string data)
    {
        Console.WriteLine($"Creating record using LINQ: {data}");
        // LINQ-based create logic would go here
    }

    public string Read(int id)
    {
        Console.WriteLine($"Reading record with ID {id} using LINQ");
        // LINQ-based read logic would go here
        return $"LINQ Record {id}";
    }

    public void Update(int id, string data)
    {
        Console.WriteLine($"Updating record {id} using LINQ: {data}");
        // LINQ-based update logic would go here
    }

    public void Delete(int id)
    {
        Console.WriteLine($"Deleting record {id} using LINQ");
        // LINQ-based delete logic would go here
    }
}

// Stored Procedure implementation
public class StoredProcedureDataAccess : IDataAccess
{
    public void Create(string data)
    {
        Console.WriteLine($"Creating record using stored procedure: {data}");
        // Stored procedure-based create logic would go here
    }

    public string Read(int id)
    {
        Console.WriteLine($"Reading record with ID {id} using stored procedure");
        // Stored procedure-based read logic would go here
        return $"SP Record {id}";
    }

    public void Update(int id, string data)
    {
        Console.WriteLine($"Updating record {id} using stored procedure: {data}");
        // Stored procedure-based update logic would go here
    }

    public void Delete(int id)
    {
        Console.WriteLine($"Deleting record {id} using stored procedure");
        // Stored procedure-based delete logic would go here
    }
}

// Factory class
public static class DataAccessFactory
{
    public static IDataAccess CreateDataAccess(string type)
    {
        return type?.ToLower() switch
        {
            "linq" => new LinqDataAccess(),
            "sproc" => new StoredProcedureDataAccess(),
            _ => throw new ArgumentException($"Unknown data access type: {type}")
        };
    }
}

// Main program
public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: program.exe [linq|sproc]");
            return;
        }

        try
        {
            IDataAccess dataAccess = DataAccessFactory.CreateDataAccess(args[0]);
            // Display menu and handle user choices
            bool running = true;
            while (running)
            {
                Console.WriteLine("\n=== CRUD Operations Menu ===");
                Console.WriteLine("1. Create");
                Console.WriteLine("2. Read");
                Console.WriteLine("3. Update");
                Console.WriteLine("4. Delete");
                Console.WriteLine("5. Exit");
                Console.Write("Select an option (1-5): ");
                
                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        Console.Write("Enter data to create: ");
                        string createData = Console.ReadLine();
                        dataAccess.Create(createData);
                        break;
                        
                    case "2":
                        Console.Write("Enter ID to read: ");
                        if (int.TryParse(Console.ReadLine(), out int readId))
                        {
                            string result = dataAccess.Read(readId);
                            Console.WriteLine($"Retrieved: {result}");
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID format.");
                        }
                        break;
                        
                    case "3":
                        Console.Write("Enter ID to update: ");
                        if (int.TryParse(Console.ReadLine(), out int updateId))
                        {
                            Console.Write("Enter new data: ");
                            string updateData = Console.ReadLine();
                            dataAccess.Update(updateId, updateData);
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID format.");
                        }
                        break;
                        
                    case "4":
                        Console.Write("Enter ID to delete: ");
                        if (int.TryParse(Console.ReadLine(), out int deleteId))
                        {
                            dataAccess.Delete(deleteId);
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID format.");
                        }
                        break;
                        
                    case "5":
                        running = false;
                        Console.WriteLine("Goodbye!");
                        break;
                        
                    default:
                        Console.WriteLine("Invalid option. Please select 1-5.");
                        break;
                }
            }
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}