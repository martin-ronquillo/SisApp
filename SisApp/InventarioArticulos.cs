namespace SisApp
{
    class InventarioArticulos
    {
        public int Id { get; set; }
        public string CodigoBarra { get; set; }
        public string Producto { get; set; }
        public float Stock { get; set; }
        public string Categoria { get; set; }
        public float StockBodega { get; set; }
        public float PrecioVenta { get; set; }
        public float PrecioCompra { get; set; }

        public InventarioArticulos()
        {

        }
    }
}
