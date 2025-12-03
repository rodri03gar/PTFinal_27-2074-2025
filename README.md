En la carpeta esta la base de datos pero la agregare aca tambien:
CREATE DATABASE IF NOT EXISTS inventario_db;

USE inventario_db;

CREATE TABLE IF NOT EXISTS productos (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL,
    descripcion VARCHAR(500), 
    precio DECIMAL(10, 2) NOT NULL,
    stock INT NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS ventas (
    id INT AUTO_INCREMENT PRIMARY KEY,
    fecha DATETIME NOT NULL,
    total DECIMAL(10, 2) NOT NULL
);

CREATE TABLE IF NOT EXISTS detalle_ventas (
    id INT AUTO_INCREMENT PRIMARY KEY,
    venta_id INT NOT NULL,
    producto_id INT NOT NULL,
    cantidad INT NOT NULL,
    precio_unitario DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (venta_id) REFERENCES ventas(id),
    FOREIGN KEY (producto_id) REFERENCES productos(id)
);

INSERT INTO productos (nombre, descripcion, precio, stock) VALUES
('Laptop Gamer RYZEN 5', 'Portátil de alto rendimiento con 16GB RAM.', 850.50, 5),
('Mouse Inalámbrico RGB', 'Mouse óptico con batería recargable.', 15.99, 50),
('Teclado Mecánico', 'Teclado con switches rojos, formato 60%.', 65.00, 12);
