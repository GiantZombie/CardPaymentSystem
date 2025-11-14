using System;
using System.Collections.Generic;

namespace CardPaymentSystem
{
	/// <summary>
	/// Represents a payment made using a card.
	/// </summary>
	public class Payment
	{
		/// <summary>
		/// Gets the current balance of the card.
		/// </summary>
		public decimal Balance { get; private set; }

		/// <summary>
		/// Gets or sets the date of the payment.
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// Gets or sets the type of payment.
		/// </summary>
		public string PaymentType { get; set; }

		/// <summary>
		/// Gets or sets the card number.
		/// </summary>
		public string CardNumber { get; set; }

		/// <summary>
		/// Gets or sets the card CVV.
		/// </summary>
		public string CVV { get; set; }

		/// <summary>
		/// Gets or sets the expiry date of the card.
		/// </summary>
		public DateTime ExpiryDate { get; set; }

		/// <summary>
		/// Gets the history of payments made with this card.
		/// </summary>
		public List<PaymentHistoryEntry> PaymentHistory { get; } = new List<PaymentHistoryEntry>();

		/// <summary>
		/// Initializes a new instance of the <see cref="Payment"/> class.
		/// </summary>
		/// <param name="balance">Initial balance of the card.</param>
		/// <param name="cardNumber">Card number.</param>
		/// <param name="cvv">Card CVV.</param>
		/// <param name="expiryDate">Card expiry date.</param>
		public Payment(decimal balance, string cardNumber, string cvv, DateTime expiryDate)
		{
			Balance = balance;
			Date = DateTime.Now;
			PaymentType = "Card";
			CardNumber = cardNumber;
			CVV = cvv;
			ExpiryDate = expiryDate;
		}

		/// <summary>
		/// Attempts to make a payment for the specified amount.
		/// </summary>
		/// <param name="totalAmount">The amount to pay.</param>
		/// <returns>True if payment is successful; otherwise, false.</returns>
		public bool MakePayment(decimal totalAmount)
		{
			if (Balance >= totalAmount)
			{
				Balance -= totalAmount;
				Console.WriteLine($"Processing {PaymentType} payment of {totalAmount:C} on {Date}");
				Console.WriteLine($"Card ending in {CardNumber.Substring(12)}, Expiry: {ExpiryDate:MM/yy}");
				Console.WriteLine($"Payment Successful! Remaining balance: {Balance:C}");
				PaymentHistory.Add(new PaymentHistoryEntry
				{
					Amount = totalAmount,
					Date = DateTime.Now,
					Success = true
				});
				return true;
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("not enought money on your balance, work harder bro ;))");
				Console.ResetColor();
				PaymentHistory.Add(new PaymentHistoryEntry
				{
					Amount = totalAmount,
					Date = DateTime.Now,
					Success = false
				});
				return false;
			}
		}

		/// <summary>
		/// Validates the card number and CVV.
		/// </summary>
		/// <returns>True if the card number and CVV are valid; otherwise, false.</returns>
		public bool ValidCardNumber()
		{
			return CardNumber.Length == 16 && CVV.Length == 3;
		}
	}

	/// <summary>
	/// Represents an entry in the payment history.
	/// </summary>
	public class PaymentHistoryEntry
	{
		/// <summary>
		/// Gets or sets the amount of the payment.
		/// </summary>
		public decimal Amount { get; set; }

		/// <summary>
		/// Gets or sets the date of the payment.
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the payment was successful.
		/// </summary>
		public bool Success { get; set; }
	}

	/// <summary>
	/// Represents a shopping cart for managing items and payments.
	/// </summary>
	public class ShoppingCart
	{
		/// <summary>
		/// Gets the total amount of items in the cart.
		/// </summary>
		public decimal TotalAmount { get; private set; }

		/// <summary>
		/// Gets or sets the payment method for the cart.
		/// </summary>
		public Payment? Payment { get; set; }

		/// <summary>
		/// Adds an item to the cart.
		/// </summary>
		/// <param name="price">The price of the item.</param>
		public void AddItem(double price)
		{
			TotalAmount += (decimal)price;
		}

		/// <summary>
		/// Removes an item from the cart.
		/// </summary>
		/// <param name="price">The price of the item.</param>
		public void RemoveItem(double price)
		{
			TotalAmount -= (decimal)price;
			if (TotalAmount < 0)
			{
				TotalAmount = 0;
			}
		}

		/// <summary>
		/// Attempts to checkout and process payment for the cart.
		/// </summary>
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

	/// <summary>
	/// Manages card registration and selection.
	/// </summary>
	public class CardManager
	{
		private List<Payment> registeredCards = new List<Payment>();

		/// <summary>
		/// Registers a new card.
		/// </summary>
		public void RegisterCard()
		{
			Payment? card = AddCard();
			if (card != null)
			{
				registeredCards.Add(card);
				Console.WriteLine("Card registered successfully");
			}
		}

		/// <summary>
		/// Gets a registered card selected by the user.
		/// </summary>
		/// <returns>The selected <see cref="Payment"/> instance, or null if none selected.</returns>
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

		/// <summary>
		/// Adds a new card by collecting user input.
		/// </summary>
		/// <returns>A new <see cref="Payment"/> instance if successful; otherwise, null.</returns>
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

			Console.WriteLine("Enter expiry date (MMYY):");
			string expiryInput = ReadExpiryDate();
			if (string.IsNullOrEmpty(expiryInput) || expiryInput.Length != 5)
			{
				Console.WriteLine("Expiry date cannot be empty or invalid format");
				return null;
			}
			DateTime cardExpiryDate;
			try
			{
				cardExpiryDate = DateTime.ParseExact(expiryInput, "MM/yy", null);
			}
			catch
			{
				Console.WriteLine("Invalid expiry date format");
				return null;
			}

			Console.WriteLine("Enter initial balance:");
			if (!decimal.TryParse(Console.ReadLine(), out decimal balance) || balance < 0)
			{
				Console.WriteLine("Invalid balance amount");
				return null;
			}

			return new Payment(balance, cardNumber, cvv, cardExpiryDate);
		}

		/// <summary>
		/// Reads the expiry date from user input in MM/yy format.
		/// </summary>
		/// <returns>The expiry date string in MM/yy format.</returns>
		private string ReadExpiryDate()
		{
			string result = "";
			while (true)
			{
				ConsoleKeyInfo key = Console.ReadKey(true);
				if (key.Key == ConsoleKey.Enter)
				{
					Console.WriteLine();
					break;
				}
				if (key.Key == ConsoleKey.Backspace)
				{
					if (result.Length > 0)
					{
						if (result.EndsWith("/"))
						{
							result = result.Substring(0, result.Length - 1);
						}
						else
						{
							result = result.Substring(0, result.Length - 1);
						}
						Console.Write("\b \b");
					}
					continue;
				}
				if (char.IsDigit(key.KeyChar))
				{
					if (result.Length < 2)
					{
						result += key.KeyChar;
						Console.Write(key.KeyChar);
						if (result.Length == 2)
						{
							result += "/";
							Console.Write("/");
						}
					}
					else if (result.Length < 5)
					{
						result += key.KeyChar;
						Console.Write(key.KeyChar);
					}
				}
			}
			return result;
		}
	}

	/// <summary>
	/// Entry point for the Card Payment System application.
	/// </summary>
	class Program
	{
		/// <summary>
		/// Main method for the application.
		/// </summary>
		/// <param name="args">Command-line arguments.</param>
		static void Main(string[] args)
		{
			CardManager cardManager = new CardManager();
			ShoppingCart cart = new ShoppingCart();

			while (true)
			{
				Console.WriteLine("1. Register Card");
				Console.WriteLine("2. Add Item to Cart");
				Console.WriteLine("3. Card Options (Pay/History)");
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
							Console.WriteLine("Choose option:");
							Console.WriteLine("1. Pay");
							Console.WriteLine("2. Payment History");
							Console.Write("Select: ");
							string? subOption = Console.ReadLine();
							if (subOption == "1")
							{
								cart.Payment = payment;
								cart.Checkout();
							}
							else if (subOption == "2")
							{
								if (payment.PaymentHistory.Count == 0)
								{
									Console.WriteLine("No payment history for this card.");
								}
								else
								{
									Console.WriteLine("Payment History:");
									foreach (var entry in payment.PaymentHistory)
									{
										Console.WriteLine($"{entry.Date:yyyy-MM-dd HH:mm} | Amount: {entry.Amount:C} | Success: {entry.Success}");
									}
								}
							}
							else
							{
								Console.WriteLine("Invalid option");
							}
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
