using System.Reflection.Metadata.Ecma335;
using Microsoft.Data.Sqlite;
using miproyecto;

public class PresupuestosRepository
{
    private string cadenaconexion = "Data Source =Tienda.db";
    List<Presupuestos> presupuestos = new List<Presupuestos>();

    public List<Presupuestos> GetAll()
    {
        string query = "SELECT * FROM presupuestos";
        var connection = new SqliteConnection(cadenaconexion);
        connection.Open();
        var command = new SqliteCommand(query, connection);

        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            var presupuesto = new Presupuestos
            {
                idPresupuestos = Convert.ToInt32(reader["idPresupuestos"]),
                nombreDestinatario = reader["nombreDestinatario"].ToString(),
                fechaCreacion = DateTime.Parse(reader["fechaCreacion"].ToString())
            };

            presupuestos.Add(presupuesto);

        }
        return presupuestos;
    }

    List<Presupuestos> listadoPresupustos = new List<Presupuestos>();
    public Presupuestos Crearpresupuesto(Presupuestos p)
    {
        using (var connection = new SqliteConnection(cadenaconexion))
        {
            connection.Open();

            // Insertar presupuesto principal
            var command = new SqliteCommand(
                "INSERT INTO presupuestos (nombreDestinatario, fechaCreacion) VALUES (@nombre, @fecha); SELECT last_insert_rowid();",
                connection
            );

            command.Parameters.AddWithValue("@nombre", p.nombreDestinatario);
            command.Parameters.AddWithValue("@fecha", p.fechaCreacion.ToString("yyyy-MM-dd HH:mm:ss"));

            // Obtener el ID autogenerado
            p.idPresupuestos = Convert.ToInt32(command.ExecuteScalar());

            // Insertar los detalles si los tiene
            if (p.detalles != null && p.detalles.Count > 0)
            {
                foreach (var d in p.detalles)
                {
                    var cmdDetalle = new SqliteCommand(
                        "INSERT INTO presupuestosDetalle (idPresupuesto, idProducto, cantidad) VALUES (@idPresupuesto, @idProducto, @cantidad)",
                        connection
                    );

                    cmdDetalle.Parameters.AddWithValue("@idPresupuesto", p.idPresupuestos);
                    cmdDetalle.Parameters.AddWithValue("@idProducto", d.producto.idProducto);
                    cmdDetalle.Parameters.AddWithValue("@cantidad", d.cantidad);

                    cmdDetalle.ExecuteNonQuery();
                }
            }
        }
        return p;
    }

    //Obtener detalles de un Presupuesto por su ID. (recibe un Id y devuelve un
    //Presupuesto con sus productos y cantidades)

    public Presupuestos PresupuestoId(int id)
    {
        var presupuesto = new Presupuestos();
        List<Productos> productos = new List<Productos>();

        using (var connection = new SqliteConnection(cadenaconexion))
        {
            connection.Open();
            var command = new SqliteCommand("SELECT * FROM Presupuestos WHERE idPresupuesto = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    presupuesto.idPresupuestos = Convert.ToInt32(reader["idPresupuesto"]);
                    presupuesto.nombreDestinatario = reader["nombreDestinatario"].ToString();
                    presupuesto.fechaCreacion = DateTime.Parse(reader["fechaCreacion"].ToString());
                }
            }

            var cmdDetalles = new SqliteCommand(
                @"SELECT d.cantidad ,p.idProducto, p.descripcion , p.precio
                FROM PresupuestosDetalle d 
                JOIN productos p ON d.idProducto = p.idProducto 
                WHERE d.idPresupuesto = p.idPresupuesto", connection);
            cmdDetalles.Parameters.AddWithValue("@idPresupuesto", id);

            using (var reader = cmdDetalles.ExecuteReader())
            {
                presupuesto.detalles.Add(new PresupuestosDetalles(
                    new Productos
                    {
                        idProducto = Convert.ToInt32(reader["idProducto"]),
                        descripcion = reader["descripcion"].ToString(),
                        precio = Convert.ToInt32(reader["precio"])
                    },
                        Convert.ToInt32(reader["cantidad"])
                    ));
            }

        }
        return presupuesto;
    }
    // Eliminar un presupuesto (y sus detalles)
    public void EliminarPresupuesto(int id)
    {
        using (var connection = new SqliteConnection(cadenaconexion))
        {
            connection.Open();

            // Borrar detalles primero
            var cmdDetalles = new SqliteCommand("DELETE FROM PresupuestosDetalles WHERE idPresupuesto = @id", connection);
            cmdDetalles.Parameters.AddWithValue("@id", id);
            cmdDetalles.ExecuteNonQuery();

            // Borrar el presupuesto
            var cmdPresupuesto = new SqliteCommand("DELETE FROM Presupuestos WHERE idPresupuesto = @id", connection);
            cmdPresupuesto.Parameters.AddWithValue("@id", id);
            cmdPresupuesto.ExecuteNonQuery();
        }
    }
    public bool EliminarPresupuestos(int id)
{
    using (var connection = new SqliteConnection(cadenaconexion))
    {
        connection.Open();

        // 1️⃣ Borrar detalles primero
        var cmdDetalles = new SqliteCommand("DELETE FROM PresupuestosDetalles WHERE idPresupuesto = @id", connection);
        cmdDetalles.Parameters.AddWithValue("@id", id);
        cmdDetalles.ExecuteNonQuery();

        // 2️⃣ Borrar el presupuesto
        var cmdPresupuesto = new SqliteCommand("DELETE FROM Presupuestos WHERE idPresupuesto = @id", connection);
        cmdPresupuesto.Parameters.AddWithValue("@id", id);
        int filasAfectadas = cmdPresupuesto.ExecuteNonQuery();

        // Si no se borró ningún presupuesto, devolvemos false
        return filasAfectadas > 0;
    }
}


    public void AgregarProducto(int idPresupuesto, int idProducto, int cantidad)
    {
        using (var connection = new SqliteConnection(cadenaconexion))
        {
            connection.Open();

            // 1️⃣ Verificamos que el presupuesto exista
            var checkPresupuesto = new SqliteCommand(
                "SELECT COUNT(*) FROM presupuestos WHERE idPresupuestos = @id", connection);
            checkPresupuesto.Parameters.AddWithValue("@id", idPresupuesto);

            long existePresupuesto = (long)checkPresupuesto.ExecuteScalar();
            if (existePresupuesto == 0)
            {
                throw new Exception("El presupuesto especificado no existe.");
            }

            // 2️⃣ Verificamos que el producto exista
            var checkProducto = new SqliteCommand(
                "SELECT COUNT(*) FROM productos WHERE idProducto = @idProducto", connection);
            checkProducto.Parameters.AddWithValue("@idProducto", idProducto);

            long existeProducto = (long)checkProducto.ExecuteScalar();
            if (existeProducto == 0)
            {
                throw new Exception("El producto especificado no existe.");
            }

            // 3️⃣ Insertamos el detalle del presupuesto
            var command = new SqliteCommand(
                "INSERT INTO presupuestosDetalle (idPresupuesto, idProducto, cantidad) VALUES (@idPresupuesto, @idProducto, @cantidad)",
                connection
            );

            command.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);
            command.Parameters.AddWithValue("@idProducto", idProducto);
            command.Parameters.AddWithValue("@cantidad", cantidad);

            command.ExecuteNonQuery();
        }
    }

public PresupuestosDetalles AgregarProductoAlPresupuesto(int idPresupuesto, PresupuestosDetalles detalle)
{
    using (var connection = new SqliteConnection(cadenaconexion))
    {
        connection.Open();

        // 1️⃣ Validar que exista el presupuesto
        var checkPresupuesto = new SqliteCommand(
            "SELECT COUNT(*) FROM presupuestos WHERE idPresupuestos = @idPresupuesto", connection);
        checkPresupuesto.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);

        long existePresupuesto = (long)checkPresupuesto.ExecuteScalar();
        if (existePresupuesto == 0)
            return null; // no existe el presupuesto

        // 2️⃣ Validar que exista el producto
        var checkProducto = new SqliteCommand(
            "SELECT COUNT(*) FROM productos WHERE idProducto = @idProducto", connection);
        checkProducto.Parameters.AddWithValue("@idProducto", detalle.producto.idProducto);

        long existeProducto = (long)checkProducto.ExecuteScalar();
        if (existeProducto == 0)
            return null; // no existe el producto

        // 3️⃣ Insertar el detalle
        var command = new SqliteCommand(
            "INSERT INTO presupuestosDetalle (idPresupuesto, idProducto, cantidad) VALUES (@idPresupuesto, @idProducto, @cantidad)",
            connection
        );

        command.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);
        command.Parameters.AddWithValue("@idProducto", detalle.producto.idProducto);
        command.Parameters.AddWithValue("@cantidad", detalle.cantidad);

        command.ExecuteNonQuery();

        // 4️⃣ Devolver el detalle insertado (ya con idPresupuesto asignado)
        detalle.producto.idProducto = detalle.producto.idProducto;
        return detalle;
    }
}


}

