using System.ComponentModel.Design;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;
using Microsoft.Data.Sqlite;
using miproyecto;
using System.Linq.Expressions;
public class ProductoRepository
{
    private string cadenaconexion = "Data Source =Tienda.db"; // nuestra ruta de la db 
    List<Productos> listadoProductos = new List<Productos>();

    public  List<Productos> GetAll()
    {
        string query = "SELECT * FROM productos"; //consulta que nos trae todos los productos de la tabla productos
        var connection = new SqliteConnection(cadenaconexion); // lo que nos permite conectarnos a la db
        connection.Open(); // nos permite realizar las consultas 
        var command = new SqliteCommand(query, connection); // prepara la consulta para ejecutar la db
        
        //ExecuteReader() ejecuta la consulta SELECT y devuelve un lector de resultados (SqliteDataReader).
        using SqliteDataReader reader = command.ExecuteReader(); 

        while (reader.Read())   //recorre fila por fila los resultados.
        {
            var producto = new Productos
            {
                idProducto = Convert.ToInt32(reader["idproducto"]),
                descripcion = reader["descripcion"].ToString(),
                precio = Convert.ToInt32(reader["precio"])
            };
            listadoProductos.Add(producto);
        }
        return listadoProductos;
    }


    public Productos CrearProducto(Productos prod)
    {
        using (var connection = new SqliteConnection(cadenaconexion))
        {
            connection.Open();
            var command = new SqliteCommand(
                "INSERT INTO Productos(Descripcion , Precio) VALUES ( @descripcion, @precio)",
                connection
                );

            command.Parameters.AddWithValue("@descripcion", prod.descripcion);
            command.Parameters.AddWithValue("@precio", prod.precio);


            command.ExecuteNonQuery();
        }
        return prod;
    }

    public Productos modificarProducto(int id, Productos prod)
    {
        using (var connection = new SqliteConnection(cadenaconexion))
        {
            connection.Open();
            var command = new SqliteCommand(
                "UPDATE Productos SET Descripcion = @descripcion , Precio = @precio WHERE idProducto = @id ",
                connection
                );
            command.Parameters.AddWithValue("@descripcion", prod.descripcion);
            command.Parameters.AddWithValue("@precio", prod.precio);
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }
        return prod;

    }

    public List<Productos> ListarProductos()
    {
        return listadoProductos;
    }

public Productos DetallesProducto(int id)
{
    using (var connection = new SqliteConnection(cadenaconexion))
    {
        connection.Open();

        var command = new SqliteCommand(
            "SELECT * FROM Productos WHERE idProducto = @id",
            connection
        );

        command.Parameters.AddWithValue("@id", id);

        using SqliteDataReader reader = command.ExecuteReader();

        if (reader.Read()) // si encontró un registro
        {
            var producto = new Productos
            {
                idProducto = Convert.ToInt32(reader["idProducto"]),
                descripcion = reader["descripcion"].ToString(),
                precio = Convert.ToInt32(reader["precio"])
            };

            return producto;
        }
    }

    // si no encontró nada, devuelve null
    return null;
}


public Productos EliminarProducto(int id)
{
    Productos productoEliminado = null;

    using (var connection = new SqliteConnection(cadenaconexion))
    {
        connection.Open();

        // Primero obtenemos el producto (para poder devolverlo después)
        var selectCommand = new SqliteCommand("SELECT * FROM Productos WHERE idProducto = @id", connection);
        selectCommand.Parameters.AddWithValue("@id", id);

        using (var reader = selectCommand.ExecuteReader())
        {
            if (reader.Read())
            {
                productoEliminado = new Productos
                {
                    idProducto = Convert.ToInt32(reader["idProducto"]),
                    descripcion = reader["descripcion"].ToString(),
                    precio = Convert.ToInt32(reader["precio"])
                };
            }
            else
            {
                // Si no existe, salimos del método devolviendo null
                return null;
            }
        }

        // Ahora eliminamos el producto
        var deleteCommand = new SqliteCommand("DELETE FROM Productos WHERE idProducto = @id", connection);
        deleteCommand.Parameters.AddWithValue("@id", id);
        deleteCommand.ExecuteNonQuery();
    }

    // Devolvemos el producto que acabamos de borrar
    return productoEliminado;
}


}