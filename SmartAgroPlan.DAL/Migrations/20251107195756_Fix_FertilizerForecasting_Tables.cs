using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAgroPlan.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Fix_FertilizerForecasting_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationPlan_FertilizationPlans_FertilizationP~",
                table: "FertilizerApplicationPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationPlan_Fields_FieldId",
                table: "FertilizerApplicationPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationPlan_PlanStages_PlanStageId",
                table: "FertilizerApplicationPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationProduct_FertilizerApplicationPlan_Appl~",
                table: "FertilizerApplicationProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationProduct_FertilizerProducts_FertilizerP~",
                table: "FertilizerApplicationProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationRecord_ApplicationMethods_ApplicationM~",
                table: "FertilizerApplicationRecord");

            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationRecord_FertilizerApplicationPlan_Appli~",
                table: "FertilizerApplicationRecord");

            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationRecord_Fields_FieldId",
                table: "FertilizerApplicationRecord");

            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationRecordProduct_FertilizerApplicationRec~",
                table: "FertilizerApplicationRecordProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationRecordProduct_FertilizerProducts_Ferti~",
                table: "FertilizerApplicationRecordProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FertilizerApplicationRecordProduct",
                table: "FertilizerApplicationRecordProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FertilizerApplicationRecord",
                table: "FertilizerApplicationRecord");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FertilizerApplicationProduct",
                table: "FertilizerApplicationProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FertilizerApplicationPlan",
                table: "FertilizerApplicationPlan");

            migrationBuilder.RenameTable(
                name: "FertilizerApplicationRecordProduct",
                newName: "FertilizerApplicationRecordProducts");

            migrationBuilder.RenameTable(
                name: "FertilizerApplicationRecord",
                newName: "FertilizerApplicationRecords");

            migrationBuilder.RenameTable(
                name: "FertilizerApplicationProduct",
                newName: "FertilizerApplicationProducts");

            migrationBuilder.RenameTable(
                name: "FertilizerApplicationPlan",
                newName: "FertilizerApplicationPlans");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationRecordProduct_FertilizerProductId",
                table: "FertilizerApplicationRecordProducts",
                newName: "IX_FertilizerApplicationRecordProducts_FertilizerProductId");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationRecordProduct_ApplicationRecordId",
                table: "FertilizerApplicationRecordProducts",
                newName: "IX_FertilizerApplicationRecordProducts_ApplicationRecordId");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationRecord_RecordedDate",
                table: "FertilizerApplicationRecords",
                newName: "IX_FertilizerApplicationRecords_RecordedDate");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationRecord_FieldId_ApplicationDate",
                table: "FertilizerApplicationRecords",
                newName: "IX_FertilizerApplicationRecords_FieldId_ApplicationDate");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationRecord_ApplicationPlanId",
                table: "FertilizerApplicationRecords",
                newName: "IX_FertilizerApplicationRecords_ApplicationPlanId");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationRecord_ApplicationMethodId",
                table: "FertilizerApplicationRecords",
                newName: "IX_FertilizerApplicationRecords_ApplicationMethodId");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationProduct_FertilizerProductId",
                table: "FertilizerApplicationProducts",
                newName: "IX_FertilizerApplicationProducts_FertilizerProductId");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationProduct_ApplicationPlanId",
                table: "FertilizerApplicationProducts",
                newName: "IX_FertilizerApplicationProducts_ApplicationPlanId");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationPlan_PlanStageId",
                table: "FertilizerApplicationPlans",
                newName: "IX_FertilizerApplicationPlans_PlanStageId");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationPlan_IsCompleted",
                table: "FertilizerApplicationPlans",
                newName: "IX_FertilizerApplicationPlans_IsCompleted");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationPlan_FieldId_PlannedApplicationDate",
                table: "FertilizerApplicationPlans",
                newName: "IX_FertilizerApplicationPlans_FieldId_PlannedApplicationDate");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationPlan_FertilizationPlanId",
                table: "FertilizerApplicationPlans",
                newName: "IX_FertilizerApplicationPlans_FertilizationPlanId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FertilizerApplicationRecordProducts",
                table: "FertilizerApplicationRecordProducts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FertilizerApplicationRecords",
                table: "FertilizerApplicationRecords",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FertilizerApplicationProducts",
                table: "FertilizerApplicationProducts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FertilizerApplicationPlans",
                table: "FertilizerApplicationPlans",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationPlans_FertilizationPlans_Fertilization~",
                table: "FertilizerApplicationPlans",
                column: "FertilizationPlanId",
                principalTable: "FertilizationPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationPlans_Fields_FieldId",
                table: "FertilizerApplicationPlans",
                column: "FieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationPlans_PlanStages_PlanStageId",
                table: "FertilizerApplicationPlans",
                column: "PlanStageId",
                principalTable: "PlanStages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationProducts_FertilizerApplicationPlans_Ap~",
                table: "FertilizerApplicationProducts",
                column: "ApplicationPlanId",
                principalTable: "FertilizerApplicationPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationProducts_FertilizerProducts_Fertilizer~",
                table: "FertilizerApplicationProducts",
                column: "FertilizerProductId",
                principalTable: "FertilizerProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationRecordProducts_FertilizerApplicationRe~",
                table: "FertilizerApplicationRecordProducts",
                column: "ApplicationRecordId",
                principalTable: "FertilizerApplicationRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationRecordProducts_FertilizerProducts_Fert~",
                table: "FertilizerApplicationRecordProducts",
                column: "FertilizerProductId",
                principalTable: "FertilizerProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationRecords_ApplicationMethods_Application~",
                table: "FertilizerApplicationRecords",
                column: "ApplicationMethodId",
                principalTable: "ApplicationMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationRecords_FertilizerApplicationPlans_App~",
                table: "FertilizerApplicationRecords",
                column: "ApplicationPlanId",
                principalTable: "FertilizerApplicationPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationRecords_Fields_FieldId",
                table: "FertilizerApplicationRecords",
                column: "FieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationPlans_FertilizationPlans_Fertilization~",
                table: "FertilizerApplicationPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationPlans_Fields_FieldId",
                table: "FertilizerApplicationPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationPlans_PlanStages_PlanStageId",
                table: "FertilizerApplicationPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationProducts_FertilizerApplicationPlans_Ap~",
                table: "FertilizerApplicationProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationProducts_FertilizerProducts_Fertilizer~",
                table: "FertilizerApplicationProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationRecordProducts_FertilizerApplicationRe~",
                table: "FertilizerApplicationRecordProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationRecordProducts_FertilizerProducts_Fert~",
                table: "FertilizerApplicationRecordProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationRecords_ApplicationMethods_Application~",
                table: "FertilizerApplicationRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationRecords_FertilizerApplicationPlans_App~",
                table: "FertilizerApplicationRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerApplicationRecords_Fields_FieldId",
                table: "FertilizerApplicationRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FertilizerApplicationRecords",
                table: "FertilizerApplicationRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FertilizerApplicationRecordProducts",
                table: "FertilizerApplicationRecordProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FertilizerApplicationProducts",
                table: "FertilizerApplicationProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FertilizerApplicationPlans",
                table: "FertilizerApplicationPlans");

            migrationBuilder.RenameTable(
                name: "FertilizerApplicationRecords",
                newName: "FertilizerApplicationRecord");

            migrationBuilder.RenameTable(
                name: "FertilizerApplicationRecordProducts",
                newName: "FertilizerApplicationRecordProduct");

            migrationBuilder.RenameTable(
                name: "FertilizerApplicationProducts",
                newName: "FertilizerApplicationProduct");

            migrationBuilder.RenameTable(
                name: "FertilizerApplicationPlans",
                newName: "FertilizerApplicationPlan");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationRecords_RecordedDate",
                table: "FertilizerApplicationRecord",
                newName: "IX_FertilizerApplicationRecord_RecordedDate");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationRecords_FieldId_ApplicationDate",
                table: "FertilizerApplicationRecord",
                newName: "IX_FertilizerApplicationRecord_FieldId_ApplicationDate");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationRecords_ApplicationPlanId",
                table: "FertilizerApplicationRecord",
                newName: "IX_FertilizerApplicationRecord_ApplicationPlanId");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationRecords_ApplicationMethodId",
                table: "FertilizerApplicationRecord",
                newName: "IX_FertilizerApplicationRecord_ApplicationMethodId");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationRecordProducts_FertilizerProductId",
                table: "FertilizerApplicationRecordProduct",
                newName: "IX_FertilizerApplicationRecordProduct_FertilizerProductId");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationRecordProducts_ApplicationRecordId",
                table: "FertilizerApplicationRecordProduct",
                newName: "IX_FertilizerApplicationRecordProduct_ApplicationRecordId");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationProducts_FertilizerProductId",
                table: "FertilizerApplicationProduct",
                newName: "IX_FertilizerApplicationProduct_FertilizerProductId");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationProducts_ApplicationPlanId",
                table: "FertilizerApplicationProduct",
                newName: "IX_FertilizerApplicationProduct_ApplicationPlanId");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationPlans_PlanStageId",
                table: "FertilizerApplicationPlan",
                newName: "IX_FertilizerApplicationPlan_PlanStageId");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationPlans_IsCompleted",
                table: "FertilizerApplicationPlan",
                newName: "IX_FertilizerApplicationPlan_IsCompleted");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationPlans_FieldId_PlannedApplicationDate",
                table: "FertilizerApplicationPlan",
                newName: "IX_FertilizerApplicationPlan_FieldId_PlannedApplicationDate");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerApplicationPlans_FertilizationPlanId",
                table: "FertilizerApplicationPlan",
                newName: "IX_FertilizerApplicationPlan_FertilizationPlanId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FertilizerApplicationRecord",
                table: "FertilizerApplicationRecord",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FertilizerApplicationRecordProduct",
                table: "FertilizerApplicationRecordProduct",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FertilizerApplicationProduct",
                table: "FertilizerApplicationProduct",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FertilizerApplicationPlan",
                table: "FertilizerApplicationPlan",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationPlan_FertilizationPlans_FertilizationP~",
                table: "FertilizerApplicationPlan",
                column: "FertilizationPlanId",
                principalTable: "FertilizationPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationPlan_Fields_FieldId",
                table: "FertilizerApplicationPlan",
                column: "FieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationPlan_PlanStages_PlanStageId",
                table: "FertilizerApplicationPlan",
                column: "PlanStageId",
                principalTable: "PlanStages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationProduct_FertilizerApplicationPlan_Appl~",
                table: "FertilizerApplicationProduct",
                column: "ApplicationPlanId",
                principalTable: "FertilizerApplicationPlan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationProduct_FertilizerProducts_FertilizerP~",
                table: "FertilizerApplicationProduct",
                column: "FertilizerProductId",
                principalTable: "FertilizerProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationRecord_ApplicationMethods_ApplicationM~",
                table: "FertilizerApplicationRecord",
                column: "ApplicationMethodId",
                principalTable: "ApplicationMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationRecord_FertilizerApplicationPlan_Appli~",
                table: "FertilizerApplicationRecord",
                column: "ApplicationPlanId",
                principalTable: "FertilizerApplicationPlan",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationRecord_Fields_FieldId",
                table: "FertilizerApplicationRecord",
                column: "FieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationRecordProduct_FertilizerApplicationRec~",
                table: "FertilizerApplicationRecordProduct",
                column: "ApplicationRecordId",
                principalTable: "FertilizerApplicationRecord",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerApplicationRecordProduct_FertilizerProducts_Ferti~",
                table: "FertilizerApplicationRecordProduct",
                column: "FertilizerProductId",
                principalTable: "FertilizerProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
