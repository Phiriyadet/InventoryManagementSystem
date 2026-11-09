-- =============================================
-- KKF Inventory Database
-- Create Tables
-- =============================================
USE [KKF];


GO
-- 1. GOODS_NAME (สินค้า)
CREATE TABLE GOODS_NAME (
    Goods_id   VARCHAR (10)   DEFAULT '' NOT NULL,
    Goods_name NVARCHAR (30)  DEFAULT '' NOT NULL,
    cost_unit  NUMERIC (8, 2) DEFAULT 0 NOT NULL,
    CONSTRAINT PK_GOODS_NAME PRIMARY KEY (Goods_id),
    CONSTRAINT CK_GOODS_NAME_Goods_id_Length
        CHECK (LEN(Goods_id) = 10)
);

-- 2. CUS_NAME (ลูกค้า)
-- หมายเหตุ: ตารางนี้ IT KIG จะสร้างให้
-- ใช้ CREATE TABLE นี้เฉพาะกรณีทดสอบในเครื่องตัวเองเท่านั้น
-- CREATE TABLE CUS_NAME (
--     Cus_id    VARCHAR(5)   NOT NULL DEFAULT '',
--     Cus_name  NVARCHAR(30) NOT NULL DEFAULT '',
--     CONSTRAINT PK_CUS_NAME
--         PRIMARY KEY (Cus_id),
--     CONSTRAINT CK_CUS_NAME_Cus_id_Length
--        CHECK (LEN(Cus_id) = 5),
--     CONSTRAINT CK_CUS_NAME_NotEmpty
--         CHECK (Cus_name <> '')
-- );
-- 3. H_ORDER (Order Header)
CREATE TABLE H_ORDER (
    Order_no   INT         IDENTITY (1, 1) NOT NULL,
    Cus_id     VARCHAR (5) DEFAULT '' NOT NULL,
    Order_Date DATETIME    DEFAULT '2000-01-01' NOT NULL,
    CONSTRAINT PK_H_ORDER PRIMARY KEY (Order_no),
    CONSTRAINT FK_H_ORDER_CUS FOREIGN KEY (Cus_id) REFERENCES CUS_NAME (Cus_id)
);

-- 4. D_ORDER (Order Detail)
CREATE TABLE D_ORDER (
    Order_no  INT             DEFAULT 0 NOT NULL,
    Goods_id  VARCHAR (10)    DEFAULT '' NOT NULL,
    Ord_date  DATETIME        DEFAULT '2000-01-01' NOT NULL,
    Fin_date  DATETIME        NULL, --ต้องเป็น NULL ถึงจะทำตามโจทย์ได้
    Amount    NUMERIC (10, 2) DEFAULT 0 NOT NULL,
    COST_UNIT NUMERIC (8, 2)  DEFAULT 0 NOT NULL,
    TOT_PRC   NUMERIC (10, 2) DEFAULT 0 NOT NULL,
    CONSTRAINT PK_D_ORDER PRIMARY KEY (Order_no, Goods_id),
    CONSTRAINT FK_D_ORDER_H FOREIGN KEY (Order_no) REFERENCES H_ORDER (Order_no) ON DELETE CASCADE,
    CONSTRAINT FK_D_ORDER_G FOREIGN KEY (Goods_id) REFERENCES GOODS_NAME (Goods_id)
);

-- 5. M_ORDER (Order Master - ส่งสินค้าแล้ว)
CREATE TABLE M_ORDER (
    Cus_id   VARCHAR (5)     DEFAULT '' NOT NULL,
    Goods_id VARCHAR (10)    DEFAULT '' NOT NULL,
    Doc_date DATETIME        DEFAULT '2000-01-01' NOT NULL,
    Ord_date DATETIME        DEFAULT '2000-01-01' NOT NULL,
    Fin_date DATETIME        DEFAULT '2000-01-01' NOT NULL,
    Sys_date DATETIME        DEFAULT '2000-01-01' NOT NULL,
    Amount   NUMERIC (10, 2) DEFAULT 0 NOT NULL,
    cost_tot NUMERIC (10, 2) DEFAULT 0 NOT NULL,
    CONSTRAINT PK_M_ORDER PRIMARY KEY (Cus_id, Goods_id, Doc_date, Ord_date, Fin_date)
);