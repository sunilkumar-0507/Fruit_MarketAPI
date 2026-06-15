CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    ALTER DATABASE CHARACTER SET utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE TABLE `Categories` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `NameEn` varchar(160) CHARACTER SET utf8mb4 NOT NULL,
        `NameTa` varchar(160) CHARACTER SET utf8mb4 NOT NULL,
        `Slug` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `DescriptionEn` longtext CHARACTER SET utf8mb4 NULL,
        `DescriptionTa` longtext CHARACTER SET utf8mb4 NULL,
        `CreatedAtUtc` datetime(6) NOT NULL,
        `UpdatedAtUtc` datetime(6) NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        CONSTRAINT `PK_Categories` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE TABLE `Coupons` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `Code` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `DiscountAmount` decimal(18,2) NOT NULL,
        `MinimumOrderAmount` decimal(18,2) NULL,
        `StartsAtUtc` datetime(6) NOT NULL,
        `EndsAtUtc` datetime(6) NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedAtUtc` datetime(6) NOT NULL,
        `UpdatedAtUtc` datetime(6) NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        CONSTRAINT `PK_Coupons` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE TABLE `Roles` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `Name` varchar(60) CHARACTER SET utf8mb4 NOT NULL,
        `CreatedAtUtc` datetime(6) NOT NULL,
        `UpdatedAtUtc` datetime(6) NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        CONSTRAINT `PK_Roles` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE TABLE `Users` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `FullName` varchar(120) CHARACTER SET utf8mb4 NOT NULL,
        `Email` varchar(256) CHARACTER SET utf8mb4 NOT NULL,
        `PhoneNumber` longtext CHARACTER SET utf8mb4 NULL,
        `PasswordHash` longtext CHARACTER SET utf8mb4 NOT NULL,
        `EmailConfirmed` tinyint(1) NOT NULL,
        `EmailVerificationToken` longtext CHARACTER SET utf8mb4 NULL,
        `PasswordResetToken` longtext CHARACTER SET utf8mb4 NULL,
        `PasswordResetTokenExpiresAtUtc` datetime(6) NULL,
        `OtpCodeHash` longtext CHARACTER SET utf8mb4 NULL,
        `OtpExpiresAtUtc` datetime(6) NULL,
        `CreatedAtUtc` datetime(6) NOT NULL,
        `UpdatedAtUtc` datetime(6) NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        CONSTRAINT `PK_Users` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE TABLE `Products` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `NameEn` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `NameTa` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `Slug` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `DescriptionEn` longtext CHARACTER SET utf8mb4 NULL,
        `DescriptionTa` longtext CHARACTER SET utf8mb4 NULL,
        `Price` decimal(18,2) NOT NULL,
        `StockQuantity` int NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CategoryId` char(36) COLLATE ascii_general_ci NOT NULL,
        `CreatedAtUtc` datetime(6) NOT NULL,
        `UpdatedAtUtc` datetime(6) NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        CONSTRAINT `PK_Products` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Products_Categories_CategoryId` FOREIGN KEY (`CategoryId`) REFERENCES `Categories` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE TABLE `Addresses` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `UserId` char(36) COLLATE ascii_general_ci NOT NULL,
        `Line1` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Line2` longtext CHARACTER SET utf8mb4 NULL,
        `City` longtext CHARACTER SET utf8mb4 NOT NULL,
        `State` longtext CHARACTER SET utf8mb4 NOT NULL,
        `PostalCode` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Country` longtext CHARACTER SET utf8mb4 NOT NULL,
        `IsDefault` tinyint(1) NOT NULL,
        `CreatedAtUtc` datetime(6) NOT NULL,
        `UpdatedAtUtc` datetime(6) NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        CONSTRAINT `PK_Addresses` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Addresses_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE TABLE `Cart` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `UserId` char(36) COLLATE ascii_general_ci NOT NULL,
        `CreatedAtUtc` datetime(6) NOT NULL,
        `UpdatedAtUtc` datetime(6) NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        CONSTRAINT `PK_Cart` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Cart_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE TABLE `RefreshTokens` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `TokenHash` longtext CHARACTER SET utf8mb4 NOT NULL,
        `ExpiresAtUtc` datetime(6) NOT NULL,
        `RevokedAtUtc` datetime(6) NULL,
        `UserId` char(36) COLLATE ascii_general_ci NOT NULL,
        `CreatedAtUtc` datetime(6) NOT NULL,
        `UpdatedAtUtc` datetime(6) NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        CONSTRAINT `PK_RefreshTokens` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_RefreshTokens_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE TABLE `UserRoles` (
        `RolesId` char(36) COLLATE ascii_general_ci NOT NULL,
        `UsersId` char(36) COLLATE ascii_general_ci NOT NULL,
        CONSTRAINT `PK_UserRoles` PRIMARY KEY (`RolesId`, `UsersId`),
        CONSTRAINT `FK_UserRoles_Roles_RolesId` FOREIGN KEY (`RolesId`) REFERENCES `Roles` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_UserRoles_Users_UsersId` FOREIGN KEY (`UsersId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE TABLE `ProductImages` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `Url` varchar(1000) CHARACTER SET utf8mb4 NOT NULL,
        `AltText` longtext CHARACTER SET utf8mb4 NULL,
        `IsPrimary` tinyint(1) NOT NULL,
        `ProductId` char(36) COLLATE ascii_general_ci NOT NULL,
        `CreatedAtUtc` datetime(6) NOT NULL,
        `UpdatedAtUtc` datetime(6) NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        CONSTRAINT `PK_ProductImages` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ProductImages_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE TABLE `Reviews` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `ProductId` char(36) COLLATE ascii_general_ci NOT NULL,
        `UserId` char(36) COLLATE ascii_general_ci NOT NULL,
        `Rating` int NOT NULL,
        `Comment` longtext CHARACTER SET utf8mb4 NULL,
        `CreatedAtUtc` datetime(6) NOT NULL,
        `UpdatedAtUtc` datetime(6) NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        CONSTRAINT `PK_Reviews` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Reviews_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_Reviews_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE TABLE `WishlistItems` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `UserId` char(36) COLLATE ascii_general_ci NOT NULL,
        `ProductId` char(36) COLLATE ascii_general_ci NOT NULL,
        `CreatedAtUtc` datetime(6) NOT NULL,
        `UpdatedAtUtc` datetime(6) NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        CONSTRAINT `PK_WishlistItems` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_WishlistItems_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_WishlistItems_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE TABLE `Orders` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `OrderNumber` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `UserId` char(36) COLLATE ascii_general_ci NOT NULL,
        `Status` int NOT NULL,
        `Subtotal` decimal(18,2) NOT NULL,
        `Discount` decimal(18,2) NOT NULL,
        `Total` decimal(18,2) NOT NULL,
        `TrackingNumber` longtext CHARACTER SET utf8mb4 NULL,
        `CouponId` char(36) COLLATE ascii_general_ci NULL,
        `ShippingAddressId` char(36) COLLATE ascii_general_ci NULL,
        `CreatedAtUtc` datetime(6) NOT NULL,
        `UpdatedAtUtc` datetime(6) NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        CONSTRAINT `PK_Orders` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Orders_Addresses_ShippingAddressId` FOREIGN KEY (`ShippingAddressId`) REFERENCES `Addresses` (`Id`),
        CONSTRAINT `FK_Orders_Coupons_CouponId` FOREIGN KEY (`CouponId`) REFERENCES `Coupons` (`Id`),
        CONSTRAINT `FK_Orders_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE TABLE `CartItems` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `CartId` char(36) COLLATE ascii_general_ci NOT NULL,
        `ProductId` char(36) COLLATE ascii_general_ci NOT NULL,
        `Quantity` int NOT NULL,
        `CreatedAtUtc` datetime(6) NOT NULL,
        `UpdatedAtUtc` datetime(6) NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        CONSTRAINT `PK_CartItems` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_CartItems_Cart_CartId` FOREIGN KEY (`CartId`) REFERENCES `Cart` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_CartItems_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE TABLE `OrderItems` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `OrderId` char(36) COLLATE ascii_general_ci NOT NULL,
        `ProductId` char(36) COLLATE ascii_general_ci NOT NULL,
        `ProductName` longtext CHARACTER SET utf8mb4 NOT NULL,
        `UnitPrice` decimal(18,2) NOT NULL,
        `Quantity` int NOT NULL,
        `CreatedAtUtc` datetime(6) NOT NULL,
        `UpdatedAtUtc` datetime(6) NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        CONSTRAINT `PK_OrderItems` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_OrderItems_Orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_OrderItems_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    INSERT INTO `Categories` (`Id`, `CreatedAtUtc`, `DescriptionEn`, `DescriptionTa`, `IsDeleted`, `NameEn`, `NameTa`, `Slug`, `UpdatedAtUtc`)
    VALUES ('33333333-3333-3333-3333-333333333333', TIMESTAMP '2026-05-28 00:00:00', 'Fresh seasonal fruits', 'புதிய பருவ பழங்கள்', FALSE, 'Fruits', 'பழங்கள்', 'fruits', NULL),
    ('44444444-4444-4444-4444-444444444444', TIMESTAMP '2026-05-28 00:00:00', 'Farm fresh vegetables', 'பண்ணை புதிய காய்கறிகள்', FALSE, 'Vegetables', 'காய்கறிகள்', 'vegetables', NULL);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    INSERT INTO `Coupons` (`Id`, `Code`, `CreatedAtUtc`, `DiscountAmount`, `EndsAtUtc`, `IsActive`, `IsDeleted`, `MinimumOrderAmount`, `StartsAtUtc`, `UpdatedAtUtc`)
    VALUES ('99999999-9999-9999-9999-999999999999', 'WELCOME10', TIMESTAMP '2026-05-28 00:00:00', 10.0, TIMESTAMP '2031-05-28 00:00:00', TRUE, FALSE, 100.0, TIMESTAMP '2026-05-28 00:00:00', NULL);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    INSERT INTO `Roles` (`Id`, `CreatedAtUtc`, `IsDeleted`, `Name`, `UpdatedAtUtc`)
    VALUES ('11111111-1111-1111-1111-111111111111', TIMESTAMP '2026-05-28 00:00:00', FALSE, 'Admin', NULL),
    ('22222222-2222-2222-2222-222222222222', TIMESTAMP '2026-05-28 00:00:00', FALSE, 'Customer', NULL);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    INSERT INTO `Products` (`Id`, `CategoryId`, `CreatedAtUtc`, `DescriptionEn`, `DescriptionTa`, `IsActive`, `IsDeleted`, `NameEn`, `NameTa`, `Price`, `Slug`, `StockQuantity`, `UpdatedAtUtc`)
    VALUES ('55555555-5555-5555-5555-555555555555', '33333333-3333-3333-3333-333333333333', TIMESTAMP '2026-05-28 00:00:00', 'Sweet premium mangoes', 'இனிப்பு தரமான மாம்பழங்கள்', TRUE, FALSE, 'Alphonso Mango', 'அல்போன்சோ மாம்பழம்', 180.0, 'alphonso-mango', 100, NULL),
    ('66666666-6666-6666-6666-666666666666', '33333333-3333-3333-3333-333333333333', TIMESTAMP '2026-05-28 00:00:00', 'Fresh bananas by dozen', 'புதிய வாழைப்பழங்கள்', TRUE, FALSE, 'Nendran Banana', 'நேந்திரன் வாழைப்பழம்', 70.0, 'nendran-banana', 250, NULL);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    INSERT INTO `ProductImages` (`Id`, `AltText`, `CreatedAtUtc`, `IsDeleted`, `IsPrimary`, `ProductId`, `UpdatedAtUtc`, `Url`)
    VALUES ('77777777-7777-7777-7777-777777777777', 'Mangoes', TIMESTAMP '2026-05-28 00:00:00', FALSE, TRUE, '55555555-5555-5555-5555-555555555555', NULL, 'https://images.unsplash.com/photo-1553279768-865429fa0078'),
    ('88888888-8888-8888-8888-888888888888', 'Bananas', TIMESTAMP '2026-05-28 00:00:00', FALSE, TRUE, '66666666-6666-6666-6666-666666666666', NULL, 'https://images.unsplash.com/photo-1603833665858-e61d17a86224');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE INDEX `IX_Addresses_UserId` ON `Addresses` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_Cart_UserId` ON `Cart` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_CartItems_CartId_ProductId` ON `CartItems` (`CartId`, `ProductId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE INDEX `IX_CartItems_ProductId` ON `CartItems` (`ProductId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_Categories_Slug` ON `Categories` (`Slug`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_Coupons_Code` ON `Coupons` (`Code`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE INDEX `IX_OrderItems_OrderId` ON `OrderItems` (`OrderId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE INDEX `IX_OrderItems_ProductId` ON `OrderItems` (`ProductId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE INDEX `IX_Orders_CouponId` ON `Orders` (`CouponId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_Orders_OrderNumber` ON `Orders` (`OrderNumber`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE INDEX `IX_Orders_ShippingAddressId` ON `Orders` (`ShippingAddressId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE INDEX `IX_Orders_UserId` ON `Orders` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE INDEX `IX_ProductImages_ProductId` ON `ProductImages` (`ProductId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE INDEX `IX_Products_CategoryId` ON `Products` (`CategoryId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_Products_Slug` ON `Products` (`Slug`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE INDEX `IX_RefreshTokens_UserId` ON `RefreshTokens` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_Reviews_ProductId_UserId` ON `Reviews` (`ProductId`, `UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE INDEX `IX_Reviews_UserId` ON `Reviews` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_Roles_Name` ON `Roles` (`Name`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE INDEX `IX_UserRoles_UsersId` ON `UserRoles` (`UsersId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_Users_Email` ON `Users` (`Email`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE INDEX `IX_WishlistItems_ProductId` ON `WishlistItems` (`ProductId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_WishlistItems_UserId_ProductId` ON `WishlistItems` (`UserId`, `ProductId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260530170348_InitialCreate') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260530170348_InitialCreate', '9.0.5');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260603152100_AddProductDetailFields') THEN

    ALTER TABLE `Products` ADD `AboutEn` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260603152100_AddProductDetailFields') THEN

    ALTER TABLE `Products` ADD `AboutTa` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260603152100_AddProductDetailFields') THEN

    ALTER TABLE `Products` ADD `BenefitsEn` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260603152100_AddProductDetailFields') THEN

    ALTER TABLE `Products` ADD `BenefitsTa` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260603152100_AddProductDetailFields') THEN

    ALTER TABLE `Products` ADD `UsageEn` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260603152100_AddProductDetailFields') THEN

    ALTER TABLE `Products` ADD `UsageTa` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260603152100_AddProductDetailFields') THEN

    UPDATE `Products` SET `AboutEn` = NULL, `AboutTa` = NULL, `BenefitsEn` = NULL, `BenefitsTa` = NULL, `UsageEn` = NULL, `UsageTa` = NULL
    WHERE `Id` = '55555555-5555-5555-5555-555555555555';
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260603152100_AddProductDetailFields') THEN

    UPDATE `Products` SET `AboutEn` = NULL, `AboutTa` = NULL, `BenefitsEn` = NULL, `BenefitsTa` = NULL, `UsageEn` = NULL, `UsageTa` = NULL
    WHERE `Id` = '66666666-6666-6666-6666-666666666666';
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260603152100_AddProductDetailFields') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260603152100_AddProductDetailFields', '9.0.5');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260615121224_AddFarmersAndBaskets') THEN

    CREATE TABLE `Baskets` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `Description` varchar(1000) CHARACTER SET utf8mb4 NOT NULL,
        `Price` decimal(18,2) NOT NULL,
        `Images` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Items` varchar(1000) CHARACTER SET utf8mb4 NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedAtUtc` datetime(6) NOT NULL,
        `UpdatedAtUtc` datetime(6) NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        CONSTRAINT `PK_Baskets` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260615121224_AddFarmersAndBaskets') THEN

    CREATE TABLE `Farmers` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `Name` varchar(160) CHARACTER SET utf8mb4 NOT NULL,
        `Village` varchar(160) CHARACTER SET utf8mb4 NULL,
        `Produce` varchar(300) CHARACTER SET utf8mb4 NULL,
        `WeeklySupplyKg` int NULL,
        `Rating` double NULL,
        `Phone` varchar(40) CHARACTER SET utf8mb4 NULL,
        `IsActive` tinyint(1) NOT NULL,
        `CreatedAtUtc` datetime(6) NOT NULL,
        `UpdatedAtUtc` datetime(6) NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        CONSTRAINT `PK_Farmers` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260615121224_AddFarmersAndBaskets') THEN

    INSERT INTO `Baskets` (`Id`, `CreatedAtUtc`, `Description`, `Images`, `IsActive`, `IsDeleted`, `Items`, `Name`, `Price`, `UpdatedAtUtc`)
    VALUES ('ba000001-0000-0000-0000-000000000001', TIMESTAMP '2026-05-28 00:00:00', 'Handcrafted festival hamper with premium mangoes, bananas, and pomegranates.', '["/images/categories/fruit-baskets.jpg","/images/products/mangoes.jpeg","/images/categories/p-pomegranate.jpg"]', TRUE, FALSE, 'Mango × 4, Banana × 6, Pomegranate × 2', 'Pongal Festival Basket', 1450.0, NULL),
    ('ba000002-0000-0000-0000-000000000002', TIMESTAMP '2026-05-28 00:00:00', 'Imported tropical selection — rambutan, dragon fruit, and mangosteen.', '["/images/products/wa2-rambutan.jpeg","/images/products/wa2-dragon-fruit.jpeg","/images/products/wa2-mangosteen.jpeg","/images/categories/p-pomegranate.jpg"]', TRUE, FALSE, 'Rambutan × 6, Dragon Fruit × 2, Mangosteen × 3', 'Exotic Mix Combo', 850.0, NULL);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260615121224_AddFarmersAndBaskets') THEN

    INSERT INTO `Farmers` (`Id`, `CreatedAtUtc`, `IsActive`, `IsDeleted`, `Name`, `Phone`, `Produce`, `Rating`, `UpdatedAtUtc`, `Village`, `WeeklySupplyKg`)
    VALUES ('fa000001-0000-0000-0000-000000000001', TIMESTAMP '2026-05-28 00:00:00', TRUE, FALSE, 'Murugesan P.', '+91 94433 00001', 'Mango, Banana', 4.9000000000000004, NULL, 'Tenkasi', 800),
    ('fa000002-0000-0000-0000-000000000002', TIMESTAMP '2026-05-28 00:00:00', TRUE, FALSE, 'Rajan T.', '+91 94433 00002', 'Guava, Papaya', 4.7000000000000002, NULL, 'Courtallam', 450),
    ('fa000003-0000-0000-0000-000000000003', TIMESTAMP '2026-05-28 00:00:00', TRUE, FALSE, 'Selvam K.', '+91 94433 00003', 'Banana, Jackfruit', 4.7999999999999998, NULL, 'Alangulam', 600),
    ('fa000004-0000-0000-0000-000000000004', TIMESTAMP '2026-05-28 00:00:00', FALSE, FALSE, 'Lakshmi A.', '+91 94433 00004', 'Pomegranate, Grapes', 4.5999999999999996, NULL, 'Kadayanallur', 300),
    ('fa000005-0000-0000-0000-000000000005', TIMESTAMP '2026-05-28 00:00:00', TRUE, FALSE, 'Kumar M.', '+91 94433 00005', 'Watermelon, Pineapple', 4.5, NULL, 'Sankarankovil', 500),
    ('fa000006-0000-0000-0000-000000000006', TIMESTAMP '2026-05-28 00:00:00', TRUE, FALSE, 'Pandian S.', '+91 94433 00006', 'Dry Fruits, Seasonal', 4.7000000000000002, NULL, 'Tirunelveli', 250);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260615121224_AddFarmersAndBaskets') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260615121224_AddFarmersAndBaskets', '9.0.5');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

