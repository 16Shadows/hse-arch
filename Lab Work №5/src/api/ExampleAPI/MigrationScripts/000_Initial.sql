CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE filials (
    "ID" integer GENERATED BY DEFAULT AS IDENTITY,
    "Name" text NOT NULL,
    CONSTRAINT "PK_filials" PRIMARY KEY ("ID")
);

CREATE TABLE housing_types (
    "ID" integer GENERATED BY DEFAULT AS IDENTITY,
    "Name" text NOT NULL,
    CONSTRAINT "PK_housing_types" PRIMARY KEY ("ID")
);

CREATE TABLE settlements (
    "ID" integer GENERATED BY DEFAULT AS IDENTITY,
    "Name" text NOT NULL,
    "FilialID" integer NULL,
    CONSTRAINT "PK_settlements" PRIMARY KEY ("ID"),
    CONSTRAINT "FK_settlements_filials_FilialID" FOREIGN KEY ("FilialID") REFERENCES filials ("ID")
);

CREATE TABLE teos (
    "ID" integer GENERATED BY DEFAULT AS IDENTITY,
    "Name" text NOT NULL,
    "Author" text NOT NULL,
    "DateCreated" timestamp with time zone NOT NULL,
    "DateUpdated" timestamp with time zone NOT NULL,
    "FilialID" integer NULL,
    "HousingTypeID" integer NULL,
    CONSTRAINT "PK_teos" PRIMARY KEY ("ID"),
    CONSTRAINT "FK_teos_filials_FilialID" FOREIGN KEY ("FilialID") REFERENCES filials ("ID"),
    CONSTRAINT "FK_teos_housing_types_HousingTypeID" FOREIGN KEY ("HousingTypeID") REFERENCES housing_types ("ID")
);

CREATE TABLE "SettlementTeo" (
    "SettlementsID" integer NOT NULL,
    "TeoID" integer NOT NULL,
    CONSTRAINT "PK_SettlementTeo" PRIMARY KEY ("SettlementsID", "TeoID"),
    CONSTRAINT "FK_SettlementTeo_settlements_SettlementsID" FOREIGN KEY ("SettlementsID") REFERENCES settlements ("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_SettlementTeo_teos_TeoID" FOREIGN KEY ("TeoID") REFERENCES teos ("ID") ON DELETE CASCADE
);

CREATE INDEX "IX_settlements_FilialID" ON settlements ("FilialID");

CREATE INDEX "IX_SettlementTeo_TeoID" ON "SettlementTeo" ("TeoID");

CREATE INDEX "IX_teos_FilialID" ON teos ("FilialID");

CREATE INDEX "IX_teos_HousingTypeID" ON teos ("HousingTypeID");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250216072652_Initial', '7.0.20');

COMMIT;

