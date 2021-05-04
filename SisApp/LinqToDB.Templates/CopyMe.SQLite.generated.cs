//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by T4Model template for T4 (https://github.com/linq2db/linq2db).
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------

#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Mapping;

namespace DataModels
{
	/// <summary>
	/// Database       : SisAppCompact
	/// Data Source    : SisAppCompact
	/// Server Version : 3.24.0
	/// </summary>
	public partial class SisAppCompactDB : LinqToDB.Data.DataConnection
	{
		public ITable<Cashier>          Cashiers          { get { return this.GetTable<Cashier>(); } }
		public ITable<Category>         Categories        { get { return this.GetTable<Category>(); } }
		public ITable<Customer>         Customers         { get { return this.GetTable<Customer>(); } }
		public ITable<Egress>           Egresses          { get { return this.GetTable<Egress>(); } }
		public ITable<EgressProduct>    EgressProducts    { get { return this.GetTable<EgressProduct>(); } }
		public ITable<Product>          Products          { get { return this.GetTable<Product>(); } }
		public ITable<ProductsPurchase> ProductsPurchases { get { return this.GetTable<ProductsPurchase>(); } }
		public ITable<ProductsReceipt>  ProductsReceipts  { get { return this.GetTable<ProductsReceipt>(); } }
		public ITable<ProductsSale>     ProductsSales     { get { return this.GetTable<ProductsSale>(); } }
		public ITable<ProductsStore>    ProductsStores    { get { return this.GetTable<ProductsStore>(); } }
		public ITable<Provider>         Providers         { get { return this.GetTable<Provider>(); } }
		public ITable<Purchase>         Purchases         { get { return this.GetTable<Purchase>(); } }
		public ITable<Receipt>          Receipts          { get { return this.GetTable<Receipt>(); } }
		public ITable<Rol>              Rols              { get { return this.GetTable<Rol>(); } }
		public ITable<Sale>             Sales             { get { return this.GetTable<Sale>(); } }
		public ITable<Store>            Stores            { get { return this.GetTable<Store>(); } }
		public ITable<TradeMark>        TradeMarks        { get { return this.GetTable<TradeMark>(); } }
		public ITable<User>             Users             { get { return this.GetTable<User>(); } }

		public SisAppCompactDB()
		{
			InitDataContext();
			InitMappingSchema();
		}

		public SisAppCompactDB(string configuration)
			: base(configuration)
		{
			InitDataContext();
			InitMappingSchema();
		}

		public SisAppCompactDB(LinqToDbConnectionOptions options)
			: base(options)
		{
			InitDataContext();
			InitMappingSchema();
		}

		partial void InitDataContext  ();
		partial void InitMappingSchema();
	}

	[Table("Cashiers")]
	public partial class Cashier
	{
		[PrimaryKey, Identity] public long   Id          { get; set; } // integer
		[Column,     Nullable] public string CheckerName { get; set; } // text(30)
		[Column,     Nullable] public string Direction   { get; set; } // text(75)
		[Column,     Nullable] public long?  StoreId     { get; set; } // integer

		#region Associations

		/// <summary>
		/// FK_Sales_0_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="CashierId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Sale> Sales { get; set; }

		/// <summary>
		/// FK_Cashiers_0_0
		/// </summary>
		[Association(ThisKey="StoreId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_Cashiers_0_0", BackReferenceName="Cashiers")]
		public Store Store { get; set; }

		#endregion
	}

	[Table("Categories")]
	public partial class Category
	{
		[PrimaryKey, Identity] public long   Id           { get; set; } // integer
		[Column,     NotNull ] public string CategoryName { get; set; } // text(100)

		#region Associations

		/// <summary>
		/// FK_Products_1_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="CategoryId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Product> Products { get; set; }

		#endregion
	}

	[Table("Customers")]
	public partial class Customer
	{
		[PrimaryKey, Identity   ] public long   Id          { get; set; } // integer
		[Column,     NotNull    ] public string Ci          { get; set; } // text(13)
		[Column,     NotNull    ] public string Name        { get; set; } // text(25)
		[Column,     NotNull    ] public string LastName    { get; set; } // text(25)
		[Column,        Nullable] public string HomeAddress { get; set; } // text(175)
		[Column,        Nullable] public string Email       { get; set; } // text(100)
		[Column,        Nullable] public string Telephone   { get; set; } // text(12)

		#region Associations

		/// <summary>
		/// FK_Sales_1_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="CustomerId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Sale> Sales { get; set; }

		#endregion
	}

	[Table("Egress")]
	public partial class Egress
	{
		[PrimaryKey, Identity   ] public long    Id               { get; set; } // integer
		[Column,     NotNull    ] public string  Type             { get; set; } // text(20)
		[Column,        Nullable] public string  EgressDate       { get; set; } // text(12)
		[Column,        Nullable] public long?   StoreId          { get; set; } // integer
		[Column,        Nullable] public double? TotalPriceEgress { get; set; } // real
		[Column,        Nullable] public string  EgressCode       { get; set; } // text(7)

		#region Associations

		/// <summary>
		/// FK_EgressProducts_0_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="EgressId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<EgressProduct> EgressProducts { get; set; }

		/// <summary>
		/// FK_Egress_0_0
		/// </summary>
		[Association(ThisKey="StoreId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_Egress_0_0", BackReferenceName="Egresses")]
		public Store Store { get; set; }

		#endregion
	}

	[Table("EgressProducts")]
	public partial class EgressProduct
	{
		[PrimaryKey, Identity] public long    Id            { get; set; } // integer
		[Column,     Nullable] public long?   ProductId     { get; set; } // integer
		[Column,     Nullable] public long?   EgressId      { get; set; } // integer
		[Column,     Nullable] public double? Amount        { get; set; } // real
		[Column,     Nullable] public double? PurchasePrice { get; set; } // real

		#region Associations

		/// <summary>
		/// FK_EgressProducts_0_0
		/// </summary>
		[Association(ThisKey="EgressId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_EgressProducts_0_0", BackReferenceName="EgressProducts")]
		public Egress Egress { get; set; }

		/// <summary>
		/// FK_EgressProducts_1_0
		/// </summary>
		[Association(ThisKey="ProductId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_EgressProducts_1_0", BackReferenceName="EgressProducts")]
		public Product Product { get; set; }

		#endregion
	}

	[Table("Products")]
	public partial class Product
	{
		[PrimaryKey, Identity] public long    Id               { get; set; } // integer
		[Column,     Nullable] public string  ProductName      { get; set; } // text(100)
		[Column,     Nullable] public double? Stock            { get; set; } // real
		[Column,     Nullable] public long?   CategoryId       { get; set; } // integer
		[Column,     Nullable] public double? PucharsePrice    { get; set; } // real
		[Column,     Nullable] public double? SalePricePercent { get; set; } // real
		[Column,     Nullable] public double? SalePrice        { get; set; } // real
		[Column,     Nullable] public string  BarCode          { get; set; } // text(25)
		[Column,     Nullable] public long?   TradeMarkId      { get; set; } // integer

		#region Associations

		/// <summary>
		/// FK_Products_1_0
		/// </summary>
		[Association(ThisKey="CategoryId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_Products_1_0", BackReferenceName="Products")]
		public Category Category { get; set; }

		/// <summary>
		/// FK_EgressProducts_1_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="ProductId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<EgressProduct> EgressProducts { get; set; }

		/// <summary>
		/// FK_ProductsPurchases_0_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="ProductId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<ProductsPurchase> ProductsPurchases { get; set; }

		/// <summary>
		/// FK_ProductsReceipts_1_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="ProductId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<ProductsReceipt> ProductsReceipts { get; set; }

		/// <summary>
		/// FK_ProductsSales_0_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="ProductId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<ProductsSale> ProductsSales { get; set; }

		/// <summary>
		/// FK_ProductsStores_0_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="ProductId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<ProductsStore> ProductsStores { get; set; }

		/// <summary>
		/// FK_Products_0_0
		/// </summary>
		[Association(ThisKey="TradeMarkId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_Products_0_0", BackReferenceName="Products")]
		public TradeMark TradeMark { get; set; }

		#endregion
	}

	[Table("ProductsPurchases")]
	public partial class ProductsPurchase
	{
		[PrimaryKey, Identity] public long    Id            { get; set; } // integer
		[Column,     Nullable] public long?   PurchaseId    { get; set; } // integer
		[Column,     Nullable] public long?   ProductId     { get; set; } // integer
		[Column,     Nullable] public long?   Amount        { get; set; } // integer
		[Column,     Nullable] public double? PurchasePrice { get; set; } // real
		[Column,     Nullable] public double? TotalPrice    { get; set; } // real

		#region Associations

		/// <summary>
		/// FK_ProductsPurchases_0_0
		/// </summary>
		[Association(ThisKey="ProductId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_ProductsPurchases_0_0", BackReferenceName="ProductsPurchases")]
		public Product Product { get; set; }

		/// <summary>
		/// FK_ProductsPurchases_1_0
		/// </summary>
		[Association(ThisKey="PurchaseId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_ProductsPurchases_1_0", BackReferenceName="ProductsPurchases")]
		public Purchase Purchase { get; set; }

		#endregion
	}

	[Table("ProductsReceipts")]
	public partial class ProductsReceipt
	{
		[PrimaryKey, Identity   ] public long    Id            { get; set; } // integer
		[Column,     NotNull    ] public long    ProductId     { get; set; } // integer
		[Column,     NotNull    ] public long    ReceiptId     { get; set; } // integer
		[Column,        Nullable] public double? Amount        { get; set; } // real
		[Column,        Nullable] public double? PurchasePrice { get; set; } // real

		#region Associations

		/// <summary>
		/// FK_ProductsReceipts_1_0
		/// </summary>
		[Association(ThisKey="ProductId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_ProductsReceipts_1_0", BackReferenceName="ProductsReceipts")]
		public Product Product { get; set; }

		/// <summary>
		/// FK_ProductsReceipts_0_0
		/// </summary>
		[Association(ThisKey="ReceiptId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_ProductsReceipts_0_0", BackReferenceName="ProductsReceipts")]
		public Receipt Receipt { get; set; }

		#endregion
	}

	[Table("ProductsSales")]
	public partial class ProductsSale
	{
		[PrimaryKey, Identity] public long    Id         { get; set; } // integer
		[Column,     Nullable] public long?   SaleId     { get; set; } // integer
		[Column,     Nullable] public long?   ProductId  { get; set; } // integer
		[Column,     Nullable] public double? Amount     { get; set; } // real
		[Column,     Nullable] public double? SalePrice  { get; set; } // real
		[Column,     Nullable] public double? TotalPrice { get; set; } // real

		#region Associations

		/// <summary>
		/// FK_ProductsSales_0_0
		/// </summary>
		[Association(ThisKey="ProductId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_ProductsSales_0_0", BackReferenceName="ProductsSales")]
		public Product Product { get; set; }

		/// <summary>
		/// FK_ProductsSales_1_0
		/// </summary>
		[Association(ThisKey="SaleId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_ProductsSales_1_0", BackReferenceName="ProductsSales")]
		public Sale Sale { get; set; }

		#endregion
	}

	[Table("ProductsStores")]
	public partial class ProductsStore
	{
		[PrimaryKey, Identity   ] public long    Id               { get; set; } // integer
		[Column,        Nullable] public long?   StoreId          { get; set; } // integer
		[Column,        Nullable] public long?   ProductId        { get; set; } // integer
		[Column,        Nullable] public double? Stock            { get; set; } // real
		[Column,        Nullable] public double? SalePricePercent { get; set; } // real
		[Column,     NotNull    ] public double  PriceByUnit      { get; set; } // real
		[Column,        Nullable] public double? PurchasePrice    { get; set; } // real

		#region Associations

		/// <summary>
		/// FK_ProductsStores_0_0
		/// </summary>
		[Association(ThisKey="ProductId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_ProductsStores_0_0", BackReferenceName="ProductsStores")]
		public Product Product { get; set; }

		/// <summary>
		/// FK_ProductsStores_1_0
		/// </summary>
		[Association(ThisKey="StoreId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_ProductsStores_1_0", BackReferenceName="ProductsStores")]
		public Store Store { get; set; }

		#endregion
	}

	[Table("Providers")]
	public partial class Provider
	{
		[PrimaryKey, Identity   ] public long   Id                { get; set; } // integer
		[Column,        Nullable] public string ProviderName      { get; set; } // text(75)
		[Column,     NotNull    ] public string Ruc               { get; set; } // text(13)
		[Column,        Nullable] public string City              { get; set; } // text(25)
		[Column,        Nullable] public string Address           { get; set; } // text(300)
		[Column,        Nullable] public string TelephoneOne      { get; set; } // text(12)
		[Column,        Nullable] public string TelephoneTwo      { get; set; } // text(12)
		[Column,        Nullable] public string WebSite           { get; set; } // text(800)
		[Column,        Nullable] public string SalesManName      { get; set; } // text(50)
		[Column,        Nullable] public string SalesManTelephone { get; set; } // text(12)
		[Column,        Nullable] public string SalesManEmail     { get; set; } // text(75)
		[Column,        Nullable] public string Bank              { get; set; } // text(100)
		[Column,        Nullable] public string AccountType       { get; set; } // text(75)
		[Column,        Nullable] public string AccountName       { get; set; } // text(100)
		[Column,        Nullable] public string AccountNumber     { get; set; } // text(24)

		#region Associations

		/// <summary>
		/// FK_Purchases_1_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="ProviderId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Purchase> Purchases { get; set; }

		#endregion
	}

	[Table("Purchases")]
	public partial class Purchase
	{
		[PrimaryKey, Identity   ] public long    Id           { get; set; } // integer
		[Column,        Nullable] public string  PurchaseCode { get; set; } // text(100)
		[Column,        Nullable] public string  PurchaseDate { get; set; } // text(50)
		[Column,        Nullable] public double? TotalPrice   { get; set; } // real
		[Column,        Nullable] public double? SubPrice     { get; set; } // real
		[Column,        Nullable] public double? Tax          { get; set; } // real
		[Column,        Nullable] public double? Discount     { get; set; } // real
		[Column,     NotNull    ] public long    ProviderId   { get; set; } // integer
		[Column,        Nullable] public long?   StoreId      { get; set; } // integer

		#region Associations

		/// <summary>
		/// FK_ProductsPurchases_1_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="PurchaseId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<ProductsPurchase> ProductsPurchases { get; set; }

		/// <summary>
		/// FK_Purchases_1_0
		/// </summary>
		[Association(ThisKey="ProviderId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_Purchases_1_0", BackReferenceName="Purchases")]
		public Provider Provider { get; set; }

		/// <summary>
		/// FK_Purchases_0_0
		/// </summary>
		[Association(ThisKey="StoreId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_Purchases_0_0", BackReferenceName="Purchases")]
		public Store Store { get; set; }

		#endregion
	}

	[Table("Receipts")]
	public partial class Receipt
	{
		[PrimaryKey, Identity   ] public long    Id                { get; set; } // integer
		[Column,     NotNull    ] public string  Type              { get; set; } // text(max)
		[Column,        Nullable] public string  ReceiptDate       { get; set; } // text(12)
		[Column,        Nullable] public long?   StoreId           { get; set; } // integer
		[Column,        Nullable] public double? TotalPriceReceipt { get; set; } // real
		[Column,        Nullable] public string  ReceiptCode       { get; set; } // text(7)

		#region Associations

		/// <summary>
		/// FK_ProductsReceipts_0_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="ReceiptId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<ProductsReceipt> ProductsReceipts { get; set; }

		/// <summary>
		/// FK_Receipts_0_0
		/// </summary>
		[Association(ThisKey="StoreId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_Receipts_0_0", BackReferenceName="Receipts")]
		public Store Store { get; set; }

		#endregion
	}

	[Table("Rols")]
	public partial class Rol
	{
		[Column(),      PrimaryKey, NotNull] public long   Id        { get; set; } // integer
		[Column("Rol"),             NotNull] public string RolColumn { get; set; } // text(25)

		#region Associations

		/// <summary>
		/// FK_Users_0_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="RoleId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<User> Users { get; set; }

		#endregion
	}

	[Table("Sales")]
	public partial class Sale
	{
		[PrimaryKey, Identity] public long    Id            { get; set; } // integer
		[Column,     Nullable] public long?   UserId        { get; set; } // integer
		[Column,     Nullable] public long?   CustomerId    { get; set; } // integer
		[Column,     Nullable] public string  SaleDate      { get; set; } // text(30)
		[Column,     Nullable] public double? TotalPrice    { get; set; } // real
		[Column,     Nullable] public double? SubPrice      { get; set; } // real
		[Column,     Nullable] public double? Tax           { get; set; } // real
		[Column,     Nullable] public double? Discount      { get; set; } // real
		[Column,     Nullable] public double? Cash          { get; set; } // real
		[Column,     Nullable] public double? RemainingCash { get; set; } // real
		[Column,     Nullable] public long?   CashierId     { get; set; } // integer

		#region Associations

		/// <summary>
		/// FK_Sales_0_0
		/// </summary>
		[Association(ThisKey="CashierId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_Sales_0_0", BackReferenceName="Sales")]
		public Cashier Cashier { get; set; }

		/// <summary>
		/// FK_Sales_1_0
		/// </summary>
		[Association(ThisKey="CustomerId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_Sales_1_0", BackReferenceName="Sales")]
		public Customer Customer { get; set; }

		/// <summary>
		/// FK_ProductsSales_1_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="SaleId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<ProductsSale> ProductsSales { get; set; }

		/// <summary>
		/// FK_Sales_2_0
		/// </summary>
		[Association(ThisKey="UserId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_Sales_2_0", BackReferenceName="Sales")]
		public User User { get; set; }

		#endregion
	}

	[Table("Stores")]
	public partial class Store
	{
		[PrimaryKey, Identity   ] public long   Id             { get; set; } // integer
		[Column,     NotNull    ] public string StoreName      { get; set; } // text(100)
		[Column,        Nullable] public string Direction      { get; set; } // text(250)
		[Column,     NotNull    ] public string Ruc            { get; set; } // text(13)
		[Column,        Nullable] public string Telephone      { get; set; } // text(12)
		[Column,        Nullable] public string OtherTelephone { get; set; } // text(12)
		[Column,        Nullable] public string Email          { get; set; } // text(75)

		#region Associations

		/// <summary>
		/// FK_Cashiers_0_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="StoreId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Cashier> Cashiers { get; set; }

		/// <summary>
		/// FK_Egress_0_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="StoreId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Egress> Egresses { get; set; }

		/// <summary>
		/// FK_ProductsStores_1_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="StoreId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<ProductsStore> ProductsStores { get; set; }

		/// <summary>
		/// FK_Purchases_0_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="StoreId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Purchase> Purchases { get; set; }

		/// <summary>
		/// FK_Receipts_0_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="StoreId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Receipt> Receipts { get; set; }

		#endregion
	}

	[Table("TradeMarks")]
	public partial class TradeMark
	{
		[PrimaryKey, Identity] public long   Id       { get; set; } // integer
		[Column,     Nullable] public string MarkName { get; set; } // text(50)

		#region Associations

		/// <summary>
		/// FK_Products_0_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="TradeMarkId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Product> Products { get; set; }

		#endregion
	}

	[Table("Users")]
	public partial class User
	{
		[PrimaryKey, NotNull    ] public long   Id       { get; set; } // integer
		[Column,     NotNull    ] public string Ci       { get; set; } // text(13)
		[Column,        Nullable] public string Name     { get; set; } // text(50)
		[Column,        Nullable] public string LastName { get; set; } // text(50)
		[Column,        Nullable] public long?  RoleId   { get; set; } // integer
		[Column,        Nullable] public string LogUser  { get; set; } // text(20)
		[Column,        Nullable] public string Password { get; set; } // text(20)

		#region Associations

		/// <summary>
		/// FK_Users_0_0
		/// </summary>
		[Association(ThisKey="RoleId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="FK_Users_0_0", BackReferenceName="Users")]
		public Rol Role { get; set; }

		/// <summary>
		/// FK_Sales_2_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="UserId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Sale> Sales { get; set; }

		#endregion
	}

	public static partial class TableExtensions
	{
		public static Cashier Find(this ITable<Cashier> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Category Find(this ITable<Category> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Customer Find(this ITable<Customer> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Egress Find(this ITable<Egress> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static EgressProduct Find(this ITable<EgressProduct> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Product Find(this ITable<Product> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static ProductsPurchase Find(this ITable<ProductsPurchase> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static ProductsReceipt Find(this ITable<ProductsReceipt> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static ProductsSale Find(this ITable<ProductsSale> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static ProductsStore Find(this ITable<ProductsStore> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Provider Find(this ITable<Provider> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Purchase Find(this ITable<Purchase> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Receipt Find(this ITable<Receipt> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Rol Find(this ITable<Rol> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Sale Find(this ITable<Sale> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Store Find(this ITable<Store> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static TradeMark Find(this ITable<TradeMark> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static User Find(this ITable<User> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}
	}
}

#pragma warning restore 1591
