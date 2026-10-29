using System;
using System.Collections.Generic;
using System.Linq;

namespace InventoryManagementSystem
{
    // Product Class
    public class Product
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
    }

    // Sale Item Class (For multi-product cart in a single sale)
    public class SaleItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int QuantitySold { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => QuantitySold * UnitPrice;
    }

    // Sale Class
    public class Sale
    {
        public string InvoiceNumber { get; set; }
        public DateTime SaleDate { get; set; }
        public List<SaleItem> Items { get; set; } = new List<SaleItem>();
        public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
    }

    class Program
    {
        static List<Product> products = new List<Product>();
        static List<Sale> sales = new List<Sale>();

        static void Main(string[] args)
        {
            // কিছু ডেমো ডেটা যোগ করা হলো টেস্টিংয়ের জন্য
            SeedData();

            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("===== INVENTORY MANAGEMENT =====");
                Console.WriteLine("1. Add Product");
                Console.WriteLine("2. View All Products");
                Console.WriteLine("3. Search Product");
                Console.WriteLine("4. Update Product");
                Console.WriteLine("5. Restock Product");
                Console.WriteLine("6. Make Sale");
                Console.WriteLine("7. Sales History");
                Console.WriteLine("8. Low Stock Report");
                Console.WriteLine("9. Sales Summary");
                Console.WriteLine("10. Exit");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        AddProduct();
                        break;
                    case "2":
                        ViewProducts();
                        break;
                    case "3":
                        SearchProduct();
                        break;
                    case "4":
                        UpdateProduct();
                        break;
                    case "5":
                        RestockProduct();
                        break;
                    case "6":
                        MakeSale();
                        break;
                    case "7":
                        SalesHistory();
                        break;
                    case "8":
                        LowStockReport();
                        break;
                    case "9":
                        SalesSummary();
                        break;
                    case "10":
                        running = false;
                        Console.WriteLine("Exiting application. Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid option! Please choose between 1 to 10.");
                        break;
                }

                if (running)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void SeedData()
        {
            products.Add(new Product { ProductId = "P001", Name = "Laptop", Price = 75000m, Quantity = 10, Category = "Electronics" });
            products.Add(new Product { ProductId = "P002", Name = "Wireless Mouse", Price = 1200m, Quantity = 4, Category = "Accessories" });
            products.Add(new Product { ProductId = "P003", Name = "Mechanical Keyboard", Price = 3500m, Quantity = 3, Category = "Accessories" });
        }

        static void AddProduct()
        {
            Console.WriteLine("--- Add New Product ---");
            Console.Write("Enter Product ID: ");
            string id = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(id) || products.Any(p => p.ProductId.Equals(id, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Error: Product ID cannot be empty or already exists!");
                return;
            }

            Console.Write("Enter Product Name: ");
            string name = Console.ReadLine()?.Trim();

            Console.Write("Enter Price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price < 0)
            {
                Console.WriteLine("Error: Price cannot be negative or invalid!");
                return;
            }

            Console.Write("Enter Quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int qty) || qty < 0)
            {
                Console.WriteLine("Error: Quantity cannot be negative or invalid!");
                return;
            }

            Console.Write("Enter Category: ");
            string category = Console.ReadLine()?.Trim();

            products.Add(new Product
            {
                ProductId = id,
                Name = name,
                Price = price,
                Quantity = qty,
                Category = category
            });

            Console.WriteLine("Product added successfully!");
        }

        static void ViewProducts()
        {
            Console.WriteLine("--- All Products ---");
            if (products.Count == 0)
            {
                Console.WriteLine("No products found in inventory.");
                return;
            }

            Console.WriteLine($"{"ID",-8} | {"Name",-22} | {"Category",-15} | {"Price",-10} | {"Stock",-8}");
            Console.WriteLine(new string('-', 72));
            foreach (var p in products)
            {
                Console.WriteLine($"{p.ProductId,-8} | {p.Name,-22} | {p.Category,-15} | {p.Price,-10:C} | {p.Quantity,-8}");
            }
        }

        static void SearchProduct()
        {
            Console.WriteLine("--- Search Product ---");
            Console.Write("Enter Product ID or Name to search: ");
            string query = Console.ReadLine()?.Trim().ToLower();

            var results = products.Where(p => p.ProductId.ToLower().Contains(query) || p.Name.ToLower().Contains(query)).ToList();

            if (results.Count == 0)
            {
                Console.WriteLine("No matching products found.");
                return;
            }

            Console.WriteLine($"\nFound {results.Count} product(s):");
            Console.WriteLine($"{"ID",-8} | {"Name",-22} | {"Category",-15} | {"Price",-10} | {"Stock",-8}");
            Console.WriteLine(new string('-', 72));
            foreach (var p in results)
            {
                Console.WriteLine($"{p.ProductId,-8} | {p.Name,-22} | {p.Category,-15} | {p.Price,-10:C} | {p.Quantity,-8}");
            }
        }

        static void UpdateProduct()
        {
            Console.WriteLine("--- Update Product ---");
            Console.Write("Enter Product ID to update: ");
            string id = Console.ReadLine()?.Trim();

            var product = products.FirstOrDefault(p => p.ProductId.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (product == null)
            {
                Console.WriteLine("Product not found!");
                return;
            }

            Console.WriteLine($"Updating: {product.Name} (ID: {product.ProductId})");

            Console.Write("Enter new name (leave blank to keep current): ");
            string name = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(name)) product.Name = name;

            Console.Write("Enter new price (leave blank to keep current): ");
            string priceStr = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(priceStr))
            {
                if (decimal.TryParse(priceStr, out decimal price) && price >= 0)
                    product.Price = price;
                else
                    Console.WriteLine("Invalid price! Keeping old price.");
            }

            Console.Write("Enter new category (leave blank to keep current): ");
            string category = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(category)) product.Category = category;

            Console.WriteLine("Product updated successfully!");
        }

        static void RestockProduct()
        {
            Console.WriteLine("--- Restock Product ---");
            Console.Write("Enter Product ID to restock: ");
            string id = Console.ReadLine()?.Trim();

            var product = products.FirstOrDefault(p => p.ProductId.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (product == null)
            {
                Console.WriteLine("Product not found!");
                return;
            }

            Console.Write($"Current stock of {product.Name} is {product.Quantity}. Enter quantity to add: ");
            if (int.TryParse(Console.ReadLine(), out int addQty) && addQty > 0)
            {
                product.Quantity += addQty;
                Console.WriteLine($"Restocked successfully! New stock: {product.Quantity}");
            }
            else
            {
                Console.WriteLine("Invalid quantity entered.");
            }
        }

        static void MakeSale()
        {
            Console.WriteLine("--- Make Sale (Invoice Generation) ---");
            List<SaleItem> saleItems = new List<SaleItem>();
            bool addingItems = true;

            while (addingItems)
            {
                Console.Write("Enter Product ID to sell: ");
                string id = Console.ReadLine()?.Trim();

                var product = products.FirstOrDefault(p => p.ProductId.Equals(id, StringComparison.OrdinalIgnoreCase));
                if (product == null)
                {
                    Console.WriteLine("Product not found!");
                }
                else
                {
                    Console.Write($"Available stock for {product.Name}: {product.Quantity}. Enter quantity to sell: ");
                    if (int.TryParse(Console.ReadLine(), out int qtySold) && qtySold > 0)
                    {
                        int alreadyInCart = saleItems.Where(si => si.ProductId == product.ProductId).Sum(si => si.QuantitySold);
                        if (qtySold + alreadyInCart > product.Quantity)
                        {
                            Console.WriteLine("Error: Requested quantity exceeds available stock!");
                        }
                        else
                        {
                            saleItems.Add(new SaleItem
                            {
                                ProductId = product.ProductId,
                                ProductName = product.Name,
                                QuantitySold = qtySold,
                                UnitPrice = product.Price
                            });
                            Console.WriteLine("Item added to cart successfully.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid quantity.");
                    }
                }

                Console.Write("Add another product to this sale? (y/n): ");
                string choice = Console.ReadLine()?.Trim().ToLower();
                if (choice != "y")
                {
                    addingItems = false;
                }
            }

            if (saleItems.Count == 0)
            {
                Console.WriteLine("Sale cancelled. No items purchased.");
                return;
            }

            // Deduct stock after successful checkout preparation
            foreach (var item in saleItems)
            {
                var p = products.First(prod => prod.ProductId == item.ProductId);
                p.Quantity -= item.QuantitySold;
            }

            // Generate Unique Invoice Number and Record Sale
            string invoiceNo = "INV-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            Sale newSale = new Sale
            {
                InvoiceNumber = invoiceNo,
                SaleDate = DateTime.Now,
                Items = saleItems
            };
            sales.Add(newSale);

            // Print Invoice Receipt
            Console.WriteLine("\n================== INVOICE ==================");
            Console.WriteLine($"Invoice No: {newSale.InvoiceNumber}");
            Console.WriteLine($"Date/Time : {newSale.SaleDate}");
            Console.WriteLine(new string('-', 45));
            Console.WriteLine($"{"Item Name",-18} | {"Qty",-5} | {"Price",-8} | {"Total",-8}");
            Console.WriteLine(new string('-', 45));
            foreach (var item in newSale.Items)
            {
                Console.WriteLine($"{item.ProductName,-18} | {item.QuantitySold,-5} | {item.UnitPrice,-8:C} | {item.TotalPrice,-8:C}");
            }
            Console.WriteLine(new string('-', 45));
            Console.WriteLine($"Total Amount: {newSale.TotalAmount:C}");
            Console.WriteLine("=============================================\n");
            Console.WriteLine("Sale completed and stock updated successfully!");
        }

        static void SalesHistory()
        {
            Console.WriteLine("--- Sales History ---");
            if (sales.Count == 0)
            {
                Console.WriteLine("No sales recorded yet.");
                return;
            }

            foreach (var s in sales)
            {
                Console.WriteLine($"Invoice: {s.InvoiceNumber} | Date: {s.SaleDate} | Total: {s.TotalAmount:C}");
                Console.WriteLine("Items Purchased:");
                foreach (var item in s.Items)
                {
                    Console.WriteLine($"  - {item.ProductName} (Qty: {item.QuantitySold} x {item.UnitPrice:C} = {item.TotalPrice:C})");
                }
                Console.WriteLine(new string('-', 45));
            }
        }

        static void LowStockReport()
        {
            Console.WriteLine("--- Low Stock Report (Stock <= 5) ---");
            var lowStockProducts = products.Where(p => p.Quantity <= 5).ToList();

            if (lowStockProducts.Count == 0)
            {
                Console.WriteLine("All products have sufficient stock (> 5).");
                return;
            }

            Console.WriteLine($"{"ID",-8} | {"Name",-22} | {"Category",-15} | {"Stock",-8}");
            Console.WriteLine(new string('-', 60));
            foreach (var p in lowStockProducts)
            {
                Console.WriteLine($"{p.ProductId,-8} | {p.Name,-22} | {p.Category,-15} | {p.Quantity,-8}");
            }
        }

        static void SalesSummary()
        {
            Console.WriteLine("--- Sales Summary ---");
            if (sales.Count == 0)
            {
                Console.WriteLine("No sales data available to summarize.");
                return;
            }

            int totalSalesCount = sales.Count;
            int totalProductsSold = sales.SelectMany(s => s.Items).Sum(i => i.QuantitySold);
            decimal totalRevenue = sales.Sum(s => s.TotalAmount);

            Console.WriteLine($"Total Sales Invoices : {totalSalesCount}");
            Console.WriteLine($"Total Products Sold  : {totalProductsSold}");
            Console.WriteLine($"Total Revenue        : {totalRevenue:C}");
        }
    }
}
