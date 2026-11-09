-- =============================================
-- KKF Inventory Database
-- Create Database
-- =============================================
IF DB_ID(N'KKF') IS NULL
    BEGIN
        CREATE DATABASE [KKF];
    END