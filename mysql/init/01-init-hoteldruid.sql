-- HotelDruid Database Initialization Script
-- This script will be automatically executed when the MySQL container starts for the first time

-- Create the hoteldruid database if it doesn't exist
CREATE DATABASE IF NOT EXISTS hoteldruid CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Use the hoteldruid database
USE hoteldruid;

-- Grant all privileges to the hoteldruid user
GRANT ALL PRIVILEGES ON hoteldruid.* TO 'hoteldruid_user'@'%';
FLUSH PRIVILEGES;

-- Create a basic configuration table for HotelDruid
-- Note: HotelDruid will create its own tables during setup
CREATE TABLE IF NOT EXISTS `configurazione` (
  `idconfigurazioni` int(11) NOT NULL AUTO_INCREMENT,
  `nome_configurazione` varchar(100) NOT NULL,
  `valore_configurazione` text,
  PRIMARY KEY (`idconfigurazioni`),
  UNIQUE KEY `nome_configurazione` (`nome_configurazione`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Insert basic configuration
INSERT INTO `configurazione` (`nome_configurazione`, `valore_configurazione`) VALUES
('versione_db', '3.0.7'),
('lingua_sistema', 'ita'),
('charset', 'utf8mb4'),
('created_by_docker', 'true')
ON DUPLICATE KEY UPDATE `valore_configurazione` = VALUES(`valore_configurazione`);

-- Create logs table for application logging
CREATE TABLE IF NOT EXISTS `log_sistema` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `timestamp` timestamp DEFAULT CURRENT_TIMESTAMP,
  `livello` varchar(20) DEFAULT 'INFO',
  `messaggio` text,
  `utente` varchar(50) DEFAULT NULL,
  `ip_address` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `timestamp` (`timestamp`),
  KEY `livello` (`livello`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Insert initial log entry
INSERT INTO `log_sistema` (`livello`, `messaggio`, `utente`) VALUES
('INFO', 'Database inizializzato tramite Docker', 'system');

-- Show status
SELECT 'HotelDruid database initialized successfully' AS status;