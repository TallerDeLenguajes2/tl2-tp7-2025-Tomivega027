using miproyecto;
public class PresupuestosDetalles
{
    public Productos producto { get; set; }
    public int cantidad { get; set; }

    public PresupuestosDetalles() { }
    public PresupuestosDetalles(Productos producto, int cantidad)
    {
        this.producto = producto;
        this.cantidad = cantidad;
    }
}