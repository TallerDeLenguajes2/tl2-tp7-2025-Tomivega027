namespace miproyecto;
public class Productos
{
    public int idProducto { get; set; }
    public string descripcion { get; set; }
    public int precio { get; set; }

    public Productos() { } // Constructor vacío necesario para ADO.NET o serialización JSON

    public Productos(int idProducto, string descripcion, int precio)
    {
        this.idProducto = idProducto;
        this.descripcion = descripcion;
        this.precio = precio;
    }
}