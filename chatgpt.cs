using Microsoft.Data.Sqlite;
using miproyecto.Models;
using System;
using System.Collections.Generic;

namespace miproyecto.Repositorios
{
    public class PresupuestosRepository
    {
        private string cadenaconexion = "Data Source=tienda.db";

        // Obtener todos los presupuestos
        public List<Presupuestos> GetAll()
        {
            var presupuestos = new List<Presupuestos>();

            using (var connection = new SqliteConnection(cadenaconexion))
            {
                connection.Open();

                var command = new SqliteCommand("SELECT * FROM presupuestos", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        presupuestos.Add(new Presupuestos
                        {
                            idPresupuestos = Convert.ToInt32(reader["idPresupuesto"]),
                            nombreDestinatario = reader["nombreDestinatario"].ToString(),
                            fechaCreacion = DateTime.Parse(reader["fechaCreacion"].ToString())
                        });
                    }
                }
            }

            return presupuestos;
        }

        // Crear un presupuesto
        public Presupuestos CrearPresupuesto(Presupuestos p)
        {
            using (var connection = new SqliteConnection(cadenaconexion))
            {
                connection.Open();
                var command = new SqliteCommand(
                    "INSERT INTO presupuestos (nombreDestinatario, fechaCreacion) VALUES (@nombre, @fecha); SELECT last_insert_rowid();",
                    connection
                );
                command.Parameters.AddWithValue("@nombre", p.nombreDestinatario);
                command.Parameters.AddWithValue("@fecha", p.fechaCreacion.ToString("yyyy-MM-dd HH:mm:ss"));
                
                // Obtener el ID generado automáticamente
                p.idPresupuestos = Convert.ToInt32(command.ExecuteScalar());
            }

            return p;
        }

        // Agregar un producto al presupuesto
        public void AgregarProductoAlPresupuesto(int idPresupuesto, int idProducto, int cantidad)
        {
            using (var connection = new SqliteConnection(cadenaconexion))
            {
                connection.Open();
                var command = new SqliteCommand(
                    "INSERT INTO presupuestos_detalles (idPresupuesto, idProducto, cantidad) VALUES (@idP, @idProd, @cant)",
                    connection
                );
                command.Parameters.AddWithValue("@idP", idPresupuesto);
                command.Parameters.AddWithValue("@idProd", idProducto);
                command.Parameters.AddWithValue("@cant", cantidad);
                command.ExecuteNonQuery();
            }
        }

        // Obtener detalles de un presupuesto por ID
        public Presupuestos GetById(int id)
        {
            var presupuesto = new Presupuestos();
            presupuesto.detalles = new List<PresupuestosDetalles>();

            using (var connection = new SqliteConnection(cadenaconexion))
            {
                connection.Open();

                // Primero, traer el presupuesto
                var command = new SqliteCommand("SELECT * FROM presupuestos WHERE idPresupuesto = @id", connection);
                command.Parameters.AddWithValue("@id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        presupuesto.idPresupuestos = Convert.ToInt32(reader["idPresupuesto"]);
                        presupuesto.nombreDestinatario = reader["nombreDestinatario"].ToString();
                        presupuesto.fechaCreacion = DateTime.Parse(reader["fechaCreacion"].ToString());
                    }
                    else
                    {
                        return null; // no existe
                    }
                }

                // Después, traer los detalles
                var cmdDetalles = new SqliteCommand(
                    @"SELECT d.cantidad, p.idProducto, p.descripcion, p.precio 
                      FROM presupuestos_detalles d
                      JOIN productos p ON d.idProducto = p.idProducto
                      WHERE d.idPresupuesto = @idPresupuesto", connection);
                cmdDetalles.Parameters.AddWithValue("@idPresupuesto", id);

                using (var reader = cmdDetalles.ExecuteReader())
                {
                    while (reader.Read())
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
                var cmdDetalles = new SqliteCommand("DELETE FROM presupuestos_detalles WHERE idPresupuesto = @id", connection);
                cmdDetalles.Parameters.AddWithValue("@id", id);
                cmdDetalles.ExecuteNonQuery();

                // Borrar el presupuesto
                var cmdPresupuesto = new SqliteCommand("DELETE FROM presupuestos WHERE idPresupuesto = @id", connection);
                cmdPresupuesto.Parameters.AddWithValue("@id", id);
                cmdPresupuesto.ExecuteNonQuery();
            }
        }
    }
}
