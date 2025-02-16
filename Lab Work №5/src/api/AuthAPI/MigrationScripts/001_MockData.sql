START TRANSACTION;

INSERT INTO users
("ID", "Username")
VALUES
(0, 'user'),
(1, 'admin');

INSERT INTO _permissions
("ID", "Name")
VALUES
(0, 'teo.get'),
(1, 'teo.create"'),
(2, 'teo.delete'),
(3, 'teo.update');

INSERT INTO "PermissionUser"
("HoldersID", "PermissionsID")
VALUES
(0, 0),
(1, 0),
(1, 1),
(1, 2),
(1, 3);

COMMIT;