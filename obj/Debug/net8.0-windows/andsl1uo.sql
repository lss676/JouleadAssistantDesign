CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "Projects" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Projects" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "CreatedDate" TEXT NOT NULL,
    "Manager" TEXT NOT NULL,
    "HandoverDate" TEXT NULL
);

CREATE TABLE "CustomItems" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CustomItems" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "IsSelected" INTEGER NOT NULL,
    "ProjectId" INTEGER NULL,
    CONSTRAINT "FK_CustomItems_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES "Projects" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ElementConfigs" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ElementConfigs" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "StandardContent" TEXT NOT NULL,
    "ProjectContent" TEXT NOT NULL,
    "ProjectId" INTEGER NULL,
    CONSTRAINT "FK_ElementConfigs_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES "Projects" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_CustomItems_ProjectId" ON "CustomItems" ("ProjectId");

CREATE INDEX "IX_ElementConfigs_ProjectId" ON "ElementConfigs" ("ProjectId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250723084142_InitDatabase', '9.0.7');

DROP INDEX "IX_CustomItems_ProjectId";

CREATE TABLE "ProjectCustomItems" (
    "ProjectsId" INTEGER NOT NULL,
    "SelectedItemsId" INTEGER NOT NULL,
    CONSTRAINT "PK_ProjectCustomItems" PRIMARY KEY ("ProjectsId", "SelectedItemsId"),
    CONSTRAINT "FK_ProjectCustomItems_CustomItems_SelectedItemsId" FOREIGN KEY ("SelectedItemsId") REFERENCES "CustomItems" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProjectCustomItems_Projects_ProjectsId" FOREIGN KEY ("ProjectsId") REFERENCES "Projects" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_ProjectCustomItems_SelectedItemsId" ON "ProjectCustomItems" ("SelectedItemsId");

CREATE TABLE "ef_temp_CustomItems" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CustomItems" PRIMARY KEY AUTOINCREMENT,
    "IsSelected" INTEGER NOT NULL,
    "Name" TEXT NOT NULL
);

INSERT INTO "ef_temp_CustomItems" ("Id", "IsSelected", "Name")
SELECT "Id", "IsSelected", "Name"
FROM "CustomItems";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;
DROP TABLE "CustomItems";

ALTER TABLE "ef_temp_CustomItems" RENAME TO "CustomItems";

COMMIT;

PRAGMA foreign_keys = 1;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250723085939_AddProjectCustomItemM2M', '9.0.7');

BEGIN TRANSACTION;
CREATE TABLE "ef_temp_ElementConfigs" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ElementConfigs" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "ProjectContent" TEXT NOT NULL,
    "ProjectId" INTEGER NOT NULL,
    "StandardContent" TEXT NOT NULL,
    CONSTRAINT "FK_ElementConfigs_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES "Projects" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_ElementConfigs" ("Id", "Name", "ProjectContent", "ProjectId", "StandardContent")
SELECT "Id", "Name", "ProjectContent", IFNULL("ProjectId", 0), "StandardContent"
FROM "ElementConfigs";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;
DROP TABLE "ElementConfigs";

ALTER TABLE "ef_temp_ElementConfigs" RENAME TO "ElementConfigs";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;
CREATE INDEX "IX_ElementConfigs_ProjectId" ON "ElementConfigs" ("ProjectId");

COMMIT;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250723093611_AddElementConfigRelation', '9.0.7');

BEGIN TRANSACTION;
ALTER TABLE "ElementConfigs" ADD "CustomItemId" INTEGER NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250723100531_SyncModel', '9.0.7');

CREATE TABLE "TemplateItems" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_TemplateItems" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL
);

CREATE TABLE "TemplateElements" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_TemplateElements" PRIMARY KEY AUTOINCREMENT,
    "TemplateItemId" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "StandardContent" TEXT NOT NULL,
    CONSTRAINT "FK_TemplateElements_TemplateItems_TemplateItemId" FOREIGN KEY ("TemplateItemId") REFERENCES "TemplateItems" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_TemplateElements_TemplateItemId" ON "TemplateElements" ("TemplateItemId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250723102205_SyncModel1', '9.0.7');

DROP INDEX "IX_ElementConfigs_ProjectId";

CREATE TABLE "ef_temp_ElementConfigs" (
    "ProjectId" INTEGER NOT NULL,
    "CustomItemId" INTEGER NOT NULL,
    "Id" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "ProjectContent" TEXT NOT NULL,
    "StandardContent" TEXT NOT NULL,
    CONSTRAINT "PK_ElementConfigs" PRIMARY KEY ("ProjectId", "CustomItemId", "Id"),
    CONSTRAINT "FK_ElementConfigs_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES "Projects" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_ElementConfigs" ("ProjectId", "CustomItemId", "Id", "Name", "ProjectContent", "StandardContent")
SELECT "ProjectId", "CustomItemId", "Id", "Name", "ProjectContent", "StandardContent"
FROM "ElementConfigs";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;
DROP TABLE "ElementConfigs";

ALTER TABLE "ef_temp_ElementConfigs" RENAME TO "ElementConfigs";

COMMIT;

PRAGMA foreign_keys = 1;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250723125346_SyncModel2', '9.0.7');

BEGIN TRANSACTION;
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250723125642_AddElementConfigCompositeKey', '9.0.7');

COMMIT;

