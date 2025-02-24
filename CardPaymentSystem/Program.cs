using System;
using System.Collections.Generic;

namespace CardPaymentSystem
{
    public class Payment
    {
        public decimal Balance { get; private set; } // ბარათის ბალანსი
        public DateTime Date { get; set; }
        public string PaymentType { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public DateTime ExpiryDate { get; set; }

        public Payment(decimal balance, string cardNumber, string cvv, DateTime expiryDate)
        {
            Balance = balance;
            Date = DateTime.Now;
            PaymentType = "Card";
            CardNumber = cardNumber;
            CVV = cvv;
            ExpiryDate = expiryDate;
        }

        public bool MakePayment(decimal totalAmount)
        {
            if (Balance >= totalAmount)
            {
                Balance -= totalAmount;
                Console.WriteLine($"Processing {PaymentType} payment of {totalAmount:C} on {Date}");
                Console.WriteLine($"Card ending in {CardNumber.Substring(12)}, Expiry: {ExpiryDate:MM/yy}");
                Console.WriteLine($"Payment Successful! Remaining balance: {Balance:C}");
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("not enought money on your balance, work harder bro ;))");
                Console.ResetColor();
                return false;
            }
        }

        public bool ValidCardNumber()
        {
            return CardNumber.Length == 16 && CVV.Length == 3;
        }
    }

    public class ShoppingCart
    {
        public decimal TotalAmount { get; private set; }
        public Payment? Payment { get; set; }

        public void AddItem(double price)
        {
            TotalAmount += (decimal)price;
        }

        public void RemoveItem(double price)
        {
            TotalAmount -= (decimal)price;
            if (TotalAmount < 0)
            {
                TotalAmount = 0;
            }
        }

        public void Checkout()
        {
            if (Payment == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Payment details are missing");
                Console.ResetColor();
                return;
            }

            if (!Payment.ValidCardNumber())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid card details");
                Console.ResetColor();
                return;
            }

            if (Payment.MakePayment(TotalAmount))
            {
                TotalAmount = 0;
            }
        }
    }

    public class CardManager
    {
        private List<Payment> registeredCards = new List<Payment>();

        public void RegisterCard()
        {
            Payment? card = AddCard();
            if (card != null)
            {
                registeredCards.Add(card);
                Console.WriteLine("Card registered successfully");
            }
        }

        public Payment? GetRegisteredCard()
        {
            if (registeredCards.Count == 0)
            {
                Console.WriteLine("No registered cards available");
                return null;
            }

            Console.WriteLine("Select a card by index:");
            for (int i = 0; i < registeredCards.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Card ending in {registeredCards[i].CardNumber.Substring(12)}");
            }

            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= registeredCards.Count)
            {
                return registeredCards[index - 1];
            }

            Console.WriteLine("Invalid selection");
            return null;
        }

        public Payment? AddCard()
        {
            Console.WriteLine("Enter card number:");
            string? cardNumber = Console.ReadLine();
            if (string.IsNullOrEmpty(cardNumber))
            {
                Console.WriteLine("Card number cannot be empty");
                return null;
            }

            Console.WriteLine("Enter CVV:");
            string? cvv = Console.ReadLine();
            if (string.IsNullOrEmpty(cvv))
            {
                Console.WriteLine("CVV cannot be empty");
                return null;
            }

            Console.WriteLine("Enter expiry date (MM/yy):");
            string? expiryDate = Console.ReadLine();
            if (string.IsNullOrEmpty(expiryDate))
            {
                Console.WriteLine("Expiry date cannot be empty");
                return null;
            }
            DateTime cardExpiryDate = DateTime.ParseExact(expiryDate, "MM/yy", null);

            Console.WriteLine("Enter initial balance:");
            if (!decimal.TryParse(Console.ReadLine(), out decimal balance) || balance < 0)
            {
                Console.WriteLine("Invalid balance amount");
                return null;
            }

            return new Payment(balance, cardNumber, cvv, cardExpiryDate);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            CardManager cardManager = new CardManager();
            ShoppingCart cart = new ShoppingCart();

            while (true)
            {
                Console.WriteLine("1. Register Card");
                Console.WriteLine("2. Add Item to Cart");
                Console.WriteLine("3. Checkout");
                Console.WriteLine("4. Exit");
                Console.Write("Select an option: ");
                string? option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        cardManager.RegisterCard();
                        break;
                    case "2":
                        Console.Write("Enter item price: ");
                        if (double.TryParse(Console.ReadLine(), out double price))
                        {
                            cart.AddItem(price);
                            Console.WriteLine("Item added to cart");
                        }
                        else
                        {
                            Console.WriteLine("Invalid price");
                        }
                        break;
                    case "3":
                        Payment? payment = cardManager.GetRegisteredCard();
                        if (payment != null)
                        {
                            cart.Payment = payment;
                            cart.Checkout();
                        }
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid option");
                        break;
                }
            }
        }
    }
}