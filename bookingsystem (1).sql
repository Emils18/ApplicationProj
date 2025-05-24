-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: May 23, 2025 at 03:21 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `bookingsystem`
--

-- --------------------------------------------------------

--
-- Table structure for table `bookings`
--

CREATE TABLE `bookings` (
  `id` int(11) NOT NULL,
  `room_id` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `guest_name` varchar(255) NOT NULL,
  `contact_info` varchar(255) NOT NULL,
  `booking_date` date NOT NULL,
  `duration_days` int(11) NOT NULL,
  `total_price` decimal(10,2) NOT NULL,
  `status` varchar(50) NOT NULL DEFAULT 'Pending',
  `payment_method` varchar(50) DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_at` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `bookings`
--

INSERT INTO `bookings` (`id`, `room_id`, `user_id`, `guest_name`, `contact_info`, `booking_date`, `duration_days`, `total_price`, `status`, `payment_method`, `created_at`, `updated_at`) VALUES
(4, 2, 12, 'ewewewewe', 'emelio', '2025-05-14', 1, 100.00, 'Paid', NULL, '2025-05-23 03:32:24', '2025-05-23 03:33:16'),
(5, 2, 12, 'mio', '09433346306', '2025-05-02', 1, 100.00, 'Paid', NULL, '2025-05-23 11:02:32', '2025-05-23 11:02:44'),
(6, 3, 12, 'mondares', '123', '2025-05-01', 1, 150.00, 'Cancelled', NULL, '2025-05-23 11:41:14', '2025-05-23 11:41:31'),
(7, 3, 12, 'wer', '32', '2025-05-03', 1, 150.00, 'Pending', NULL, '2025-05-23 11:49:34', '2025-05-23 11:49:34'),
(12, 5, 12, 'ewewe', '2323', '2025-05-23', 1, 250.00, 'Cancelled', NULL, '2025-05-23 11:54:32', '2025-05-23 11:54:43'),
(13, 2, 12, 'rerf', '123', '2025-05-03', 1, 100.00, 'Pending', NULL, '2025-05-23 12:02:18', '2025-05-23 12:02:18'),
(14, 5, 12, 'eewe', '123', '2025-05-09', 1, 250.00, 'Paid', 'GCash', '2025-05-23 12:03:36', '2025-05-23 12:03:48'),
(17, 2, 21, 'e23', '123', '2025-05-08', 1, 100.00, 'Paid', 'GCash', '2025-05-23 12:40:55', '2025-05-23 12:41:09'),
(19, 2, 22, 'ewe', '23', '2025-05-08', 1, 100.00, 'Paid', 'GCash', '2025-05-23 12:53:44', '2025-05-23 12:54:09');

-- --------------------------------------------------------

--
-- Table structure for table `rooms`
--

CREATE TABLE `rooms` (
  `id` int(11) NOT NULL,
  `room_number` varchar(50) NOT NULL,
  `room_type` varchar(50) NOT NULL,
  `description` text DEFAULT NULL,
  `price` decimal(10,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `rooms`
--

INSERT INTO `rooms` (`id`, `room_number`, `room_type`, `description`, `price`) VALUES
(1, '1', 'Standard', 'A comfortable standard room.', 100.00),
(2, '2', 'Standard', 'Standard room with city view.', 100.00),
(3, '3', 'Deluxe', 'Spacious deluxe room with balcony.', 150.00),
(4, '4', 'Deluxe', 'Deluxe room with ocean view.', 160.00),
(5, '5', 'Suite', 'Luxurious suite with separate living area.', 250.00),
(6, '6', 'Standard', 'A comfortable standard room.', 100.00);

-- --------------------------------------------------------

--
-- Table structure for table `userbookings`
--

CREATE TABLE `userbookings` (
  `id` int(11) NOT NULL,
  `room_number` varchar(50) NOT NULL,
  `name` varchar(100) NOT NULL,
  `booking_date` datetime NOT NULL,
  `contact_info` varchar(100) NOT NULL,
  `room_type` varchar(50) DEFAULT NULL,
  `user_id` int(11) DEFAULT NULL,
  `duration_days` int(11) DEFAULT NULL,
  `price` decimal(10,2) DEFAULT NULL,
  `status` varchar(50) DEFAULT NULL,
  `room_id` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `userbookings`
--

INSERT INTO `userbookings` (`id`, `room_number`, `name`, `booking_date`, `contact_info`, `room_type`, `user_id`, `duration_days`, `price`, `status`, `room_id`) VALUES
(1, 'ROOM 2', 'ewewe', '2025-05-09 00:00:00', '2323', 'Standard', 12, 1, 100.00, 'Pending', NULL),
(2, '2', 'wewe', '2025-05-03 00:00:00', '123', 'Standard', 12, 1, 100.00, 'Pending', 2),
(3, '2', '12', '2025-05-08 00:00:00', 'ewe', 'Standard', 12, 1, 100.00, 'Pending', 2),
(4, '2', 'wewe', '2025-05-09 00:00:00', '123', 'Standard', 12, 1, 100.00, 'Pending', 2),
(5, '2', 'ew', '2025-05-23 00:00:00', 'e', 'Standard', 12, 1, 100.00, 'Pending', 2),
(6, '2', 'wewe', '2025-05-23 00:00:00', '123', 'Standard', 12, 1, 100.00, 'Pending', 2),
(7, '2', 'ewe', '2025-05-23 00:00:00', '2', 'Standard', 12, 1, 100.00, 'Pending', 2),
(8, '2', 'ewee', '2025-05-23 00:00:00', '123', 'Standard', 12, 1, 100.00, 'Pending', 2),
(9, '2', 'wqw', '2025-05-10 00:00:00', '23', 'Standard', 12, 1, 100.00, 'Pending', 2),
(10, '3', 'ewewe', '2025-05-23 00:00:00', '123', 'Deluxe', 12, 1, 150.00, 'Pending', 3),
(11, '3', 'emelio', '2025-05-07 00:00:00', '123', 'Deluxe', 12, 1, 150.00, 'Pending', 3),
(12, '5', 'emeoio', '2025-05-03 00:00:00', '123', 'Suite', 11, 1, 250.00, 'Pending', 5),
(13, '5', 'wqw', '2025-05-10 00:00:00', '123', 'Suite', 12, 1, 250.00, 'Pending', 5);

-- --------------------------------------------------------

--
-- Table structure for table `userlogin`
--

CREATE TABLE `userlogin` (
  `id` int(11) NOT NULL,
  `fullname` varchar(100) NOT NULL,
  `email` varchar(100) NOT NULL,
  `password` varchar(100) NOT NULL,
  `role` varchar(50) DEFAULT 'user'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `userlogin`
--

INSERT INTO `userlogin` (`id`, `fullname`, `email`, `password`, `role`) VALUES
(1, 'emelio', 'emeliomondares14@gmail.com', '12345', 'user'),
(2, 'meo', 'Emeliomondares13@gmail.com', '123', 'user'),
(3, 'emelio', 'emiliomondares15@gmail.com', '1234', 'user'),
(4, 'emelio', 'emeliomondares23@gmail.com', '12345', 'user'),
(5, 'mio', 'emiliomondares24@gmail.com', '123456', 'user'),
(6, 'HANA', 'hana@19gmail.com', '123', 'user'),
(8, 'emelio', 'Emeliomondares11@gmail.com', '123', 'user'),
(9, 'mio', 'emeliomondares16@gmail.com', '123', 'user'),
(10, 'mio ', 'emeliomondare@17gmail.com', '123', 'user'),
(11, 'emelio', 'emeliomondares22@gmail.com', '123', 'user'),
(12, 'emelio', 'emeliomodnares11@gmail.com', '123', 'user'),
(13, 'emeliomondares12', 'emelio@10gmail.com', '132', 'user'),
(21, 'ewwewe', 'ew@12gmail.com', '23', 'user'),
(22, 'e', 'e@123gmail.com', '123', 'user');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `id` int(11) NOT NULL,
  `fullname` varchar(100) NOT NULL,
  `email` varchar(100) NOT NULL,
  `password` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`id`, `fullname`, `email`, `password`) VALUES
(1, '[value-2]', '[value-3]', '[value-4]'),
(2, 'hi', 'emeliomondares12@ggmail.com', '232332'),
(11, 'User Eleven', 'user11@example.com', 'hashed_password_11'),
(12, 'User Twelve', 'user12@example.com', 'hashed_password_12'),
(21, 'ewwewe', 'ew@12gmail.com', '23'),
(22, 'e', 'e@123gmail.com', '123');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `bookings`
--
ALTER TABLE `bookings`
  ADD PRIMARY KEY (`id`),
  ADD KEY `room_id` (`room_id`),
  ADD KEY `user_id` (`user_id`);

--
-- Indexes for table `rooms`
--
ALTER TABLE `rooms`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `room_number` (`room_number`);

--
-- Indexes for table `userbookings`
--
ALTER TABLE `userbookings`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `userlogin`
--
ALTER TABLE `userlogin`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `email` (`email`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `email` (`email`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `bookings`
--
ALTER TABLE `bookings`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=20;

--
-- AUTO_INCREMENT for table `rooms`
--
ALTER TABLE `rooms`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT for table `userbookings`
--
ALTER TABLE `userbookings`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT for table `userlogin`
--
ALTER TABLE `userlogin`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=23;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=23;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `bookings`
--
ALTER TABLE `bookings`
  ADD CONSTRAINT `bookings_ibfk_1` FOREIGN KEY (`room_id`) REFERENCES `rooms` (`id`),
  ADD CONSTRAINT `bookings_ibfk_2` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
