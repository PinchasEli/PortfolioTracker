CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "Users" (
    "Id" uuid NOT NULL,
    "Email" text NOT NULL,
    "PasswordHash" text NOT NULL,
    "Role" integer NOT NULL,
    "Active" boolean NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

CREATE TABLE "Customers" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "FullName" text NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Customers" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Customers_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Portfolios" (
    "Id" uuid NOT NULL,
    "CustomerId" uuid NOT NULL,
    "Name" text NOT NULL,
    "Exchange" integer NOT NULL,
    "BaseCurrency" integer NOT NULL,
    "Active" boolean NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Portfolios" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Portfolios_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "PortfolioCashBalances" (
    "Id" uuid NOT NULL,
    "PortfolioId" uuid NOT NULL,
    "Amount" numeric NOT NULL,
    "Currency" integer NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_PortfolioCashBalances" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PortfolioCashBalances_Portfolios_PortfolioId" FOREIGN KEY ("PortfolioId") REFERENCES "Portfolios" ("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX "IX_Customers_UserId" ON "Customers" ("UserId");

CREATE UNIQUE INDEX "IX_PortfolioCashBalances_PortfolioId_Currency" ON "PortfolioCashBalances" ("PortfolioId", "Currency");

CREATE UNIQUE INDEX "IX_Portfolios_CustomerId_Name_Exchange" ON "Portfolios" ("CustomerId", "Name", "Exchange");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251225140716_InitialCreate', '8.0.0');

COMMIT;

