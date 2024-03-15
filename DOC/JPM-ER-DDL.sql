DROP DATABASE jpm_analysis_database;
CREATE DATABASE jpm_analysis_database;
USE jpm_analysis_database;
SET time_zone = "-05:00";
CREATE TABLE `profiles` (
    `id` varchar(255) PRIMARY KEY COMMENT 'Profile name in snake_case',
    `profile_name` varchar(255) UNIQUE NOT NULL,
    `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
    `updated_at` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);
CREATE TABLE `functions` (
    `id` varchar(255) PRIMARY KEY COMMENT 'Function name in snake_case',
    `function_name` varchar(255) UNIQUE NOT NULL,
    `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
    `updated_at` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `deleted_at` datetime
);
CREATE TABLE `products` (
    `id` varchar(255) PRIMARY KEY COMMENT 'Product name in snake_case',
    `product_name` varchar(255) NOT NULL,
    `sub_product` varchar(255),
    `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
    `updated_at` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `deleted_at` datetime
);
CREATE TABLE `profiles_functions` (
    `id` varchar(255) PRIMARY KEY,
    `profile_id` varchar(255) NOT NULL,
    `function_id` varchar(255) NOT NULL
);
CREATE TABLE `company_users` (
    `access_id` varchar(255) PRIMARY KEY,
    `user_name` varchar(255) NOT NULL,
    `user_status` boolean NOT NULL,
    `user_type` varchar(255) NOT NULL,
    `employee_id` varchar(255),
    `email_address` varchar(255) NOT NULL,
    `user_location` varchar(255),
    `user_country` varchar(255) NOT NULL,
    `user_logon_type` varchar(255) NOT NULL COMMENT 'RSA Token o Password',
    `user_last_logon_dt` datetime COMMENT 'Se debe hacer una conversión',
    `user_logon_status` varchar(255) NOT NULL,
    `user_group_membership` varchar(255),
    `user_role` varchar(255),
    `profile_id` varchar(255) NOT NULL,
    `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
    `updated_at` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `deleted_at` datetime
);
CREATE TABLE `user_entitlements` (
    `id` varchar(255) PRIMARY KEY,
    `access_id` varchar(255) NOT NULL,
    `product_id` varchar(255) NOT NULL,
    `account_number` varchar(255),
    `function_id` varchar(255),
    `function_type` varchar(255),
    `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
    `updated_at` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `deleted_at` datetime
);
CREATE TABLE `products_accounts` (
    `id` varchar(255) PRIMARY KEY,
    `product_id` varchar(255),
    `account_number` varchar(255),
    `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
    `updated_at` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `deleted_at` datetime DEFAULT null
);
CREATE TABLE `accounts` (
    `account_number` varchar(255) PRIMARY KEY COMMENT 'Los valores que sean null se reemplazan por un 0',
    `account_name` varchar(255) NOT NULL,
    `account_type` varchar(255) NOT NULL,
    `bank_currency` varchar(255),
    `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
    `updated_at` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `deleted_at` datetime
);
CREATE TABLE `app_users` (
    `id` varchar(255) PRIMARY KEY,
    `username` varchar(255) UNIQUE,
    `name` varchar(255),
    `app_role` ENUM ('admin', 'basic') DEFAULT ('basic'),
    `email` varchar(255),
    `password` varchar(255),
    `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
    `updated_at` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `deleted_at` datetime
);
CREATE TABLE `app_history` (
    `id` varchar(255) PRIMARY KEY,
    `app_user_id` varchar(255),
    `app_function` ENUM (
        'upload_fill',
        'upload_update',
        'add',
        'delete',
        'compare'
    ),
    `app_table` ENUM (
        'profiles',
        'functions',
        'products',
        'profiles_functions',
        'company_users',
        'user_entitlements',
        'products_accounts',
        'accounts',
        'app_users',
        'app_history',
        'report_history'
    )
);
CREATE TABLE `report_history` (
    `id` varchar(255) PRIMARY KEY,
    `app_user_id` varchar(255),
    `report_name` varchar(255),
    `report_upload_date` datetime DEFAULT (NOW())
);
CREATE UNIQUE INDEX `profiles_functions_index_0` ON `profiles_functions` (`profile_id`, `function_id`);
CREATE UNIQUE INDEX `products_accounts_index_1` ON `products_accounts` (`product_id`, `account_number`);
CREATE UNIQUE INDEX `accounts_index_2` ON `accounts` (`account_number`, `account_name`);
ALTER TABLE `profiles_functions`
ADD FOREIGN KEY (`profile_id`) REFERENCES `profiles` (`id`);
ALTER TABLE `profiles_functions`
ADD FOREIGN KEY (`function_id`) REFERENCES `functions` (`id`);
ALTER TABLE `company_users`
ADD FOREIGN KEY (`profile_id`) REFERENCES `profiles` (`id`);
ALTER TABLE `user_entitlements`
ADD FOREIGN KEY (`access_id`) REFERENCES `company_users` (`access_id`);
ALTER TABLE `user_entitlements`
ADD FOREIGN KEY (`account_number`) REFERENCES `accounts` (`account_number`);
ALTER TABLE `user_entitlements`
ADD FOREIGN KEY (`product_id`) REFERENCES `products` (`id`);
ALTER TABLE `user_entitlements`
ADD FOREIGN KEY (`function_id`) REFERENCES `functions` (`id`);
ALTER TABLE `products_accounts`
ADD FOREIGN KEY (`account_number`) REFERENCES `accounts` (`account_number`);
ALTER TABLE `products_accounts`
ADD FOREIGN KEY (`product_id`) REFERENCES `products` (`id`);
ALTER TABLE `app_history`
ADD FOREIGN KEY (`app_user_id`) REFERENCES `app_users` (`id`);
ALTER TABLE `report_history`
ADD FOREIGN KEY (`app_user_id`) REFERENCES `app_users` (`id`);