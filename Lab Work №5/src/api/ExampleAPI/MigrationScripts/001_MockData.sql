START TRANSACTION;

INSERT INTO filials
("ID", "Name")
VALUES
(0, 'Пермь ЭРТХ');

INSERT INTO settlements
("ID", "Name", "FilialID")
VALUES
(0, 'Пермь', 0),
(1, 'Краснокамск', 0);

INSERT INTO housing_types
("ID", "Name")
VALUES
(0, 'МКД');

INSERT INTO teos
("ID", "Name", "Author", "DateCreated", "DateUpdated", "FilialID", "HousingTypeID")
VALUES
(0, 'test', 'admin', now(), now(), 0, 0);

INSERT INTO "SettlementTeo"
("TeoID", "SettlementsID")
VALUES
(0, 0),
(0, 1);

COMMIT;