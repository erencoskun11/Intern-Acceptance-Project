using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTriggerAndSP_WithoutEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // --- 1. KISIM: EF CORE TARAFINDAN OTOMATİK OLUŞTURULAN DÜZELTMELER (DOKUNMA) ---
            // StudentId -> InternId dönüşümünü veritabanı kurallarına yansıtıyor.

            migrationBuilder.DropCheckConstraint(
                name: "CK_Assignments_Assignee",
                table: "Assignments");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Assignments_Assignee",
                table: "Assignments",
                sql: "(\"InternId\" IS NOT NULL AND \"EmployeeId\" IS NULL) OR (\"InternId\" IS NULL AND \"EmployeeId\" IS NOT NULL)");


            // --- 2. KISIM: BİZİM EKLEDİĞİMİZ VTYS ÖZELLİKLERİ (SQL KODLARI) ---

            // A) STORED PROCEDURE (SAKLI YORDAM) & HATA YAKALAMA
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION GetSystemStats()
                RETURNS TABLE (TotalCount integer, AvailableCount integer, StatusMsg text) 
                AS $$
                BEGIN
                    RETURN QUERY 
                    SELECT 
                        (SELECT COUNT(*) FROM ""InventoryItems"")::integer,
                        (SELECT COUNT(*) FROM ""InventoryItems"" WHERE ""Status"" = 1)::integer,
                        'Sistem Çalışıyor'::text;

                EXCEPTION WHEN OTHERS THEN
                    -- HATA YÖNETİMİ
                    RETURN QUERY SELECT 0, 0, 'Hata: ' || SQLERRM;
                END;
                $$ LANGUAGE plpgsql;
            ");

            // B) TRIGGER FUNCTION (TETİKLEYİCİ FONKSİYONU)
            // Zimmet yapıldığında InventoryItem tablosundaki 'Notes' alanını günceller.
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION update_item_note_on_assign()
                RETURNS TRIGGER AS $$
                BEGIN
                    UPDATE ""InventoryItems""
                    SET ""Notes"" = COALESCE(""Notes"", '') || ' [OTOMATIK LOG: ' || NOW() || ' tarihinde zimmetlendi.]'
                    WHERE ""Id"" = NEW.""InventoryItemId"";
                    
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;
            ");

            // C) TRIGGER TANIMI
            migrationBuilder.Sql(@"
                CREATE TRIGGER trg_UpdateNoteAfterAssign
                AFTER INSERT ON ""Assignments""
                FOR EACH ROW
                EXECUTE FUNCTION update_item_note_on_assign();
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // --- GERİ ALMA İŞLEMLERİ ---

            // 1. Bizim eklediğimiz SQL nesnelerini sil
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_UpdateNoteAfterAssign ON \"Assignments\";");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS update_item_note_on_assign;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS GetSystemStats;");

            // 2. Otomatik constraint değişikliğini geri al (InternId -> StudentId)
            migrationBuilder.DropCheckConstraint(
                name: "CK_Assignments_Assignee",
                table: "Assignments");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Assignments_Assignee",
                table: "Assignments",
                sql: "(\"StudentId\" IS NOT NULL AND \"EmployeeId\" IS NULL) OR (\"StudentId\" IS NULL AND \"EmployeeId\" IS NOT NULL)");
        }
    }
}