using miproyecto;
public class Presupuestos

{
    public int idPresupuestos { get; set; }
    public string nombreDestinatario { get; set; }
    public DateTime fechaCreacion { get; set; }
    public List<PresupuestosDetalles> detalles { get; set; }
    public Presupuestos()
    {
        detalles = new List<PresupuestosDetalles>();
    }
    public Presupuestos(int idPresupuestos, string nombreDestinatario, DateTime fechacreacion)
    {
        this.idPresupuestos = idPresupuestos;
        this.nombreDestinatario = nombreDestinatario;
        fechaCreacion = fechacreacion;
        detalles = new List<PresupuestosDetalles>();
    }

    public double MontoPresupuesto()
    {
        double suma = 0;

        foreach (var detalle in detalles)
        {
            suma += detalle.producto.precio;
        }
        return suma;
    }

    public double MontoPresupuestoConIva()
    {
        double montoPresupuesto = MontoPresupuesto();
        double iva = 0.21;
        return montoPresupuesto + (montoPresupuesto * iva);
    }

    public int CantidadProductos()
    {
        int cantidad = detalles.Count();
        return cantidad;
    }
}