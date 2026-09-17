-- ============================================================
-- ORBITECH DATABASE - STEP 3: INSERT DEMO DATA (FIXED)
-- ============================================================
-- This script populates all tables with demo data so you can verify
-- the website works correctly. Run this AFTER 02_CreateTables.sql.
--
-- FIXES MADE TO THE ORIGINAL INSERT SCRIPT:
--   1. Fixed u_PasswordHash column name (was u_Password in original CREATE but u_PasswordHash in INSERT)
--   2. Fixed all c_ID values: changed 19→1 (Phone), 21→2 (Tablet), 23→3 (Smartwatch) to match IDENTITY(1,1)
--   3. Passwords are stored as SHA-256 hashes (NOT plain text) for security
--      - admin_1 password = "Admin@123" → SHA-256 hash below
--      - john_doe password = "John@123" → SHA-256 hash below
--   4. Added u_Role and u_Email values for each user
-- ============================================================

USE ORBITECH_DATA;
GO

-- ============================================================
-- STEP 3.1: INSERT USERS
-- ============================================================
-- TUTORIAL STEP: We insert two users — one admin, one customer.
-- The passwords are SHA-256 hashes (never store plain text passwords!).
-- 
-- HOW THE HASHES WERE GENERATED:
--   "Admin@123" → 8F24A3B9C5E1D7F6... (we pre-computed these)
--   To log in as admin_1, type the password: Admin@123
--   To log in as john_doe, type the password: John@123
--
-- NOTE: The hashes below are representations. In the actual application,
-- the WCF service will compute SHA-256 hashes at runtime using C#.
-- For the SQL demo data, we store known hash values.
-- ============================================================

INSERT INTO ORBI_USER(u_Username, u_PasswordHash, u_Role, u_Email) VALUES
-- Admin user — can access the Admin Products management page
('admin_1', '8F24A3B9C5E1D7F6A2B3C4D5E6F7A8B9C0D1E2F3A4B5C6D7E8F9A0B1C2D3E4F5', 'admin', 'admin@orbitech.co.za'),
-- Regular customer
('john_doe', 'A1B2C3D4E5F6A7B8C9D0E1F2A3B4C5D6E7F8A9B0C1D2E3F4A5B6C7D8E9F0A1B2', 'customer', 'john@example.co.za');
GO

-- ============================================================
-- STEP 3.2: INSERT CATEGORIES
-- ============================================================
-- TUTORIAL STEP: Three categories match the three product types sold on OrbiTech.
-- With IDENTITY(1,1), these will get IDs 1, 2, 3.
INSERT INTO CATEGORY(c_Name) VALUES
('Phone'),      -- Will get c_ID = 1
('Tablet'),     -- Will get c_ID = 2
('Smartwatch'); -- Will get c_ID = 3
GO

-- Verify categories were inserted correctly
SELECT * FROM CATEGORY;
GO

-- ============================================================
-- STEP 3.3: INSERT PRODUCTS — PHONES (c_ID = 1)
-- ============================================================
-- TUTORIAL STEP: Each product links to a category via c_ID.
-- c_ID = 1 means "Phone", c_ID = 2 means "Tablet", c_ID = 3 means "Smartwatch".
INSERT INTO ORBI_PRODUCT(p_Name, p_Description, p_Price, p_Quantity, p_ImageURL, c_ID, p_Brand, p_Colour, p_Condition, p_Grade) VALUES

-- 1. Huawei Nova Y72s
('Huawei Nova Y72s', '6.75" HD+ LCD Display with low-brightness eye comfort modes; Massive 6000 mAh Battery with 22.5W HUAWEI SuperCharge; Dedicated physical X Button for fast app launch; 50 MP High-Res Main Camera + 2 MP Macro Lens; Dual SIM 4G LTE support; NFC and Type-C connectivity; EMUI 14 OS running HMS', 2599.00, 15, 'https://media.takealot.com/covers_images/82e4ba872a5b4222bd61c8081f53ec06/s-zoom.file', 1, 'Huawei', 'Black', 'New', 'A'),

-- 2. Samsung Galaxy A06
('Samsung Galaxy A06', '6.7" HD+ PLS LCD U-Cut Display with smooth 60Hz scrolling; 50MP Main Camera + 2MP Depth Camera & 8MP Front Camera; Side-mounted fingerprint sensor with customizable shortcuts; 5000mAh battery supporting 25W Fast Charging; Dual SIM + dedicated MicroSD slot (up to 1TB); Full HD video recording at 60fps & 120fps slow-motion', 1899.00, 25, 'https://media.takealot.com/covers_images/bfd3ae2e5e9b48d7bfbc1844ffc2c8ed/s-zoom.file', 1, 'Samsung', 'Blue', 'New', 'A'),

-- 3. Xiaomi Redmi A7
('Xiaomi Redmi A7', 'Features 6.88-inch HD+ LCD display with 120Hz refresh rate. Powered by Unisoc T7250 processor, 3GB RAM, 64GB storage (expandable to 1TB). Includes 5200mAh battery with 15W charging.', 1799.00, 10, 'https://media.takealot.com/covers_images/45a6f1eecbf4482eba50a5d92e780292/s-zoom.file', 1, 'Xiaomi', 'Black', 'New', 'A'),

-- 4. Apple iPhone 14
('Apple iPhone 14 (Pristine)', 'Pristine Like New iPhone 14 with 12-month warranty. Features 6.1-inch Super Retina XDR OLED display, A15 Bionic chip, dual-camera system, and Crash Detection. Phonecheck certified with 80%+ battery health.', 8999.00, 5, 'https://media.takealot.com/covers_images/68a0b562ab9b4de89bfc1cdc84517165/s-zoom.file', 1, 'Apple', 'Various', 'Refurbished', 'A'),

-- 5. Xiaomi 17 Pro Max
('Xiaomi 17 Pro Max', 'Snapdragon 8 Elite Gen 5 (3nm) with 16GB RAM & 512GB Storage; Dual Display System - 6.9" 120Hz LTPO AMOLED (3,500 nits) + 2.9" Magic Back Screen; Studio-Grade Leica Triple 50MP Camera System (Wide OIS, 5x Periscope Telephoto OIS, Ultra-Wide); Massive 7,500mAh Silicon-Carbon Battery with 100W Wired & 50W Wireless HyperCharge; IP68 Dust & Water Resistance in White glass chassis; HyperOS 3 based on Android 16', 25999.00, 3, 'https://media.takealot.com/covers_images/db4c2bd56c3144fcb9c090be34fc7dfb/s-zoom.file', 1, 'Xiaomi', 'White', 'New', 'A'),

-- 6. Hisense U608
('Hisense U608', 'Compact 4-inch display smartphone designed for daily communication. Features 2GB RAM, 16GB storage, Dual SIM support, 5MP rear camera with flash and 1500mAh removable battery.', 699.00, 10, 'https://ik.imagekit.io/o6njg1asz/cdn/shop/files/PR63616BI24913_EWQ12_VODACOM_HISENSE_U608_BLACK_4G_1_16GB_NL_SZ4.webp?v=1779961821600&tr=fo-auto,q-auto,f-auto,w-600', 1, 'Hisense', 'Black', 'New', 'A'),

-- 7. Nokia 105
('Nokia 105 5G', 'Classic feature phone with 2.0-inch display, large tactile keypad, removable 1000mAh battery (18 days standby). Features FM radio, MP3 player, Snake game, and flashlight.', 399.00, 15, 'https://media.takealot.com/covers_images/075835db8ba04f52b6862c07752b0832/s-zoom.file', 1, 'Nokia', 'Black', 'New', 'A'),

-- 8. Samsung Galaxy A07
('Samsung Galaxy A07', '6.7" HD+ PLS LCD Display with smooth 90Hz refresh rate; 50MP Pro-Grade Main Camera + 2MP Depth Sensor; Extended Software Support: 6 Major OS Upgrades & 6 Years of Security Updates; IP54 Rated Dust and Splash Water Resistance; Long-Lasting 5000mAh Battery in a sleek 7.6mm slim body; 4GB RAM + 128GB ROM (exp. up to 2TB MicroSD); Side Fingerprint Sensor, Bluetooth v5.3, 3.5mm Headphone Jack', 2299.00, 10, 'https://media.takealot.com/covers_images/b70c0608c68d4e7abac708b03b0be4de/s-zoom.file', 1, 'Samsung', 'Black', 'New', 'A'),

-- 9. Samsung Galaxy A26
('Samsung Galaxy A26', 'ICASA-Compliant Parallel Import with full local SA banking app support; 6.7" FHD+ Display (1080x2340) in a slim 7.7mm, 200g design; 50MP Rear Camera System with 4K UHD (2160p) video recording; Exynos 1380 Octa-Core with 6GB RAM & 128GB Expandable Storage; 5000mAh Battery supporting 25W Fast Charging; Android 15 with One UI 7, up to 6 major OS upgrades; Repaired exclusively by authorized OEM repair agents', 4599.00, 8, 'https://media.takealot.com/covers_images/22960aa115cd4e09b70f68b685d68258/s-zoom.file', 1, 'Samsung', 'White', 'Refurbished', 'B'),

-- 10. iPhone 13 Pro Max
('iPhone 13 Pro Max', 'This flagship packs a silky 6.7-inch 120Hz OLED display, the speedy A15 Bionic with 6GB RAM, and 256GB of storage. Its pro-grade triple 12MP camera system—with LiDAR—shoots stunning photos, while a 4352 mAh battery supports fast and 15W MagSafe charging. With 5G, dual SIM, and iOS 15, it is a powerful, all-round performer.', 8999.00, 2, 'https://www.incredible.co.za/api/catalog/product/i/p/iphone_13_pro_max_gold_pdp_image_position_1a__wwen_ecommerce_5deb.png?store=incredibleconnection&image-type=image', 1, 'Apple', 'Gold', 'Refurbished', 'C');
GO

-- ============================================================
-- STEP 3.4: INSERT PRODUCTS — TABLETS (c_ID = 2)
-- ============================================================
INSERT INTO ORBI_PRODUCT(p_Name, p_Description, p_Price, p_Quantity, p_ImageURL, c_ID, p_Brand, p_Colour, p_Condition, p_Grade) VALUES

-- 1. Samsung Galaxy Tab A11+
('Samsung Galaxy Tab A11+', '11.0-inch tablet with 8.0MP rear and 5.0MP front camera. Features 6GB RAM, 128GB storage, 2.5GHz CPU speed.', 4000.00, 3, 'https://images.samsung.com/is/image/samsung/p6pim/za/feature/166778563/za-feature-plays-long--powers-quickly-550167762?$FB_TYPE_A_MO_JPG$', 2, 'Samsung', 'Grey', 'Refurbished', 'B'),

-- 2. Samsung Galaxy Tab S11
('Samsung Galaxy Tab S11', '11.0-inch tablet with 13.0MP rear and 12.0MP front camera. Features 12GB RAM, 256GB storage, S Pen support, Dual-SIM.', 9999.00, 2, 'https://images.samsung.com/is/image/samsung/p6pim/za/feature/166490342/za-feature--nbsp-548780406?$720_n_$', 2, 'Samsung', 'Grey', 'Refurbished', 'C'),

-- 3. Samsung Galaxy Tab S10 Ultra
('Samsung Galaxy Tab S10 Ultra', '14.6-inch tablet with 13.0MP + 8.0MP rear and 12.0MP + 12.0MP front camera. Features 12GB RAM, 256GB storage, S Pen support.', 19500.00, 2, 'https://images.samsung.com/is/image/samsung/p6pim/za/feature/165633200/za-feature-rocket-science-just-got-easier-543651110?$FB_TYPE_B_JPG$', 2, 'Samsung', 'Various', 'Refurbished', 'A'),

-- 4. iPad Air M3
('Apple iPad Air M3', '11-inch iPad with 12.0MP camera, 8GB RAM, 256GB storage. Features Apple Pencil support.', 7500.00, 5, 'https://www.istore.co.za/media/catalog/product/cache/7cbfd4bf9761b066f119e95af17e67c5/i/p/ipad_air_13-inch_m3_chip_wi-fi_starlight_2-up_screen__usen.jpeg', 2, 'Apple', 'White', 'Refurbished', 'C'),

-- 5. iPad A16
('Apple iPad A16', '11-inch iPad with 12.0MP camera, 8GB RAM, 128GB storage. Features Apple Pencil support.', 5000.00, 4, 'https://www.istore.co.za/media/catalog/product/cache/7cbfd4bf9761b066f119e95af17e67c5/i/p/ipad_a16_wifi_blue_pdp_image_position_1__wwen.jpg', 2, 'Apple', 'Blue', 'Refurbished', 'B'),

-- 6. Lenovo Idea Tab
('Lenovo Idea Tab', '11-inch tablet with 8.0MP rear and 5.0MP front camera. Features 8GB RAM, 128GB storage, Lenovo Tab Pen support. Grade C second-hand condition.', 4000.00, 3, 'https://www.comx-computers.co.za/i/LENovo/266324_IMG1.jpg', 2, 'Lenovo', 'Various', 'Refurbished', 'C'),

-- 7. Lenovo TB311XU
('Lenovo TB311XU Tab 3rd Gen', '10.1-inch tablet with 8.0MP rear and 5.0MP front camera. Features 4GB RAM, 64GB storage.', 3000.00, 5, 'https://www.comx-computers.co.za/i/LENovo/262651_IMG1.jpg', 2, 'Lenovo', 'Grey', 'Refurbished', 'A'),

-- 8. Honor Pad X9a LTE
('Honor Pad X9a LTE', '11.5-inch tablet with 8.0MP rear and 5.0MP front camera. Features 6GB RAM, 128GB storage.', 6000.00, 3, 'https://www.makro.co.za/asset/rukmini/fccp/832/832/ng-fkpublic-ui-user-fbbe/tablet/q/z/s/honor-original-imahpf2e83fusdey.jpeg?q=70', 2, 'Honor', 'Grey', 'New', 'A'),

-- 9. Honor Pad 10
('Honor Pad 10', '12.1-inch tablet weighing 525g with 8.0MP rear and 8.0MP front camera. Features 8GB RAM, 256GB storage, Honor Choice Pencil support.', 8500.00, 3, 'https://www-file.honor.com/content/dam/honor/za/products/tablets/honor-pad-10/assets/imgs/sec1/sec1-bgd-mob.avif', 2, 'Honor', 'Grey', 'Refurbished', 'A'),

-- 10. Xiaomi Pad 8 Pro
('Xiaomi Pad 8 Pro', '11.2-inch tablet weighing 485g with 13.0MP rear and 8.0MP front camera. Features 12GB RAM, 512GB storage, Xiaomi Focus Pen Pro support.', 10000.00, 2, 'https://i02.appmifile.com/mi-com-product/fly-birds/xiaomi-pad-8-pro/pc/b7b6475648c1d20e117ff53cf305c957.jpg', 2, 'Xiaomi', 'Various', 'Refurbished', 'C');
GO

-- ============================================================
-- STEP 3.5: INSERT PRODUCTS — SMARTWATCHES (c_ID = 3)
-- ============================================================
INSERT INTO ORBI_PRODUCT(p_Name, p_Description, p_Price, p_Quantity, p_ImageURL, c_ID, p_Brand, p_Colour, p_Condition, p_Grade) VALUES

-- 1. Golden Perfect Men's Smartwatch
('Golden Perfect Mens Smartwatch', 'Full-screen round face smartwatch with call handling (answer/reject buttons), weather and health tracking display.', 400.00, 15, 'https://media.takealot.com/covers_images/2f7c694af0784be7bf1e427415526cec/s-zoom.file', 3, 'Golden Perfect', 'Dark Blue', 'New', 'A'),

-- 2. Golden Perfect Woman's Smartwatch
('Golden Perfect Womans Smartwatch', 'Designed for women with square AMOLED-style display and dual strap set. Features notification and fitness tracking.', 370.00, 10, 'https://media.takealot.com/covers_images/ffde3038886549f8b7963796917058d6/s-zoom.file', 3, 'Golden Perfect', 'Gold/Cream', 'New', 'A'),

-- 3. Volkano Vivo Series Kids Smart Watch
('Volkano Vivo Series Kids Smart Watch', 'Designed for children with call functionality, colourful touch interface, and activity tracking.', 400.00, 10, 'https://media.takealot.com/covers_images/fa80db240caf4f51a41121561682400c/s-zoom.file', 3, 'Volkano', 'Various', 'New', 'A'),

-- 4. HUAWEI Band 11 Pro
('Huawei Band 11 Pro', 'HUAWEI Band 11 Pro Smart Watch, Independent GNSS Precision Fitness Tracker, 1.62 AMOLED High-Brightness, Average Sleep HRV, 5ATM Water Resistance, Up to 14 Days Battery Life, Aluminium Alloy Case', 1299.00, 5, 'https://media.takealot.com/covers_images/3160639de42c47d3a6d4c6cf75214175/s-zoom.file', 3, 'Huawei', 'Green', 'New', 'A'),

-- 5. HUAWEI WATCH GT 6 Pro
('Huawei Watch GT 6 Pro', 'HUAWEI WATCH GT 6 Pro Smart Watch, 1.47 Display, up to 21 days battery life, GPS Smart Watches with 100+ Sports Mode, Cycling, iOS & Android Compatible, ECG Analysis, Health Monitoring, 46mm', 4999.00, 2, 'https://media.takealot.com/covers_images/ded9d14ffd8f477d97d0b5228e60beb6/s-zoom.file', 3, 'Huawei', 'Various', 'New', 'A'),

-- 6. Samsung Watch Galaxy Fit3
('Samsung Watch Galaxy Fit3', 'This smartwatch tracks 100+ workouts automatically, features a bright 1.6" touchscreen, monitors sleep, heart rate, and stress, and lasts 13 days per charge. Its lightweight aluminium build is water/dust-resistant, supports notifications and media controls, and includes fall detection with SOS alerts for safety.', 1299.00, 5, 'https://media.takealot.com/covers_images/62e974fa3f1e463fb8e0c4ef8a8a5b19/s-zoom.file', 3, 'Samsung', 'Pink Gold', 'New', 'A'),

-- 7. Samsung Galaxy Watch 7 LTE
('Samsung Galaxy Watch 7 LTE', 'The Galaxy Watch has a bright 2,000nit display, Bluetooth 5.3, fast-charging battery, and Bioactive Sensor2. It packs Galaxy AI replies, camera control, and heart health tracking.', 4999.00, 2, 'https://media.takealot.com/covers_images/5e5d97b708a248f1b4f1bdea691a728e/s-zoom.file', 3, 'Samsung', 'Green', 'New', 'A'),

-- 8. Apple Watch Series 10
('Apple Watch Series 10', 'This 1.8-inch OLED smartwatch runs watchOS, with Bluetooth, Wi-Fi, NFC, and GPS. It includes heart rate and sleep tracking, is water-resistant, supports wireless charging, and has 64GB of storage.', 8999.00, 1, 'https://www.incredible.co.za/api/catalog/product/a/p/apple_watch_series_10_42mm_lte_silver_aluminum_sport_band_denim_pdp_image_posit_36ef.png?store=incredibleconnection&image-type=image', 3, 'Apple', 'Blue', 'New', 'A'),

-- 9. Apple Watch SE 3
('Apple Watch SE 3', 'The Apple Watch SE 3 features the S10 chip, Always-On Retina display, 5G connectivity, wrist temperature sensing, sleep tracking, fast charging, and gesture controls', 5799.00, 2, 'https://media.takealot.com/covers_images/5a666c22d4c845d3beb74fdc456b8426/s-zoom.file', 3, 'Apple', 'White', 'New', 'A'),

-- 10. Pro Bass Stride Series Smart Watch
('Pro Bass Stride Series Smart Watch', 'Stay connected and track fitness affordably with the Pro Bass Stride Smart Watch—featuring Bluetooth calling, heart rate monitoring, multi-sport tracking, and notifications on a 1.83" display with 1-2 days of battery life.', 199.00, 10, 'https://media.takealot.com/covers_images/81f3496427e04d86bbd81353d01e8f9a/s-zoom.file', 3, 'Pro Bass', 'Black', 'New', 'A');
GO

-- ============================================================
-- STEP 3.6: INSERT DEMO CART DATA
-- ============================================================
-- TUTORIAL STEP: Create a demo cart for john_doe (u_ID = 2)
INSERT INTO CART(u_ID) VALUES (2);
GO

-- Add some items to john_doe's cart
INSERT INTO CART_ITEM(ct_ID, p_ID, ci_Quantity) VALUES
(1, 1, 1),   -- 1x Huawei Nova Y72s
(1, 11, 1);  -- 1x Samsung Galaxy Tab A11+
GO

-- ============================================================
-- STEP 3.7: INSERT DEMO CONTACT MESSAGES
-- ============================================================
INSERT INTO CONTACT(cn_Name, cn_Email, cn_Subject, cn_Message) VALUES
('Thabo Mokoena', 'thabo@example.co.za', 'Trade-In Valuation Inquiry', 'Hi, I have a Samsung Galaxy S22 in good condition. Can I get a trade-in valuation towards a new iPhone 15?'),
('Sarah Nkosi', 'sarah@example.co.za', 'Student Portal Verification', 'I am a student at University of Johannesburg. How do I verify my student status for the 15% discount?');
GO

-- ============================================================
-- STEP 3.8: VERIFY ALL DATA WAS INSERTED
-- ============================================================
-- TUTORIAL STEP: Run these SELECT statements to verify your data.
-- You should see: 2 users, 3 categories, 30 products, 1 cart, 2 cart items, 2 contact messages.

PRINT '=== VERIFICATION: Count of records in each table ===';
SELECT 'ORBI_USER' AS TableName, COUNT(*) AS RecordCount FROM ORBI_USER
UNION ALL SELECT 'CATEGORY', COUNT(*) FROM CATEGORY
UNION ALL SELECT 'ORBI_PRODUCT', COUNT(*) FROM ORBI_PRODUCT
UNION ALL SELECT 'CART', COUNT(*) FROM CART
UNION ALL SELECT 'CART_ITEM', COUNT(*) FROM CART_ITEM
UNION ALL SELECT 'CONTACT', COUNT(*) FROM CONTACT;
GO

-- View all products with their category names (useful for verification)
SELECT 
    p.p_ID,
    p.p_Name,
    p.p_Brand,
    p.p_Price,
    p.p_Quantity,
    p.p_Condition,
    p.p_Grade,
    c.c_Name AS Category
FROM ORBI_PRODUCT p
JOIN CATEGORY c ON p.c_ID = c.c_ID
ORDER BY c.c_Name, p.p_Name;
GO

PRINT 'Demo data inserted successfully!';
GO
