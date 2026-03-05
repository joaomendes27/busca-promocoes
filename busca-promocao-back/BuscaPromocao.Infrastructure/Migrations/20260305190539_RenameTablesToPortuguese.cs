using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuscaPromocao.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameTablesToPortuguese : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Keywords_Users_UserId",
                table: "Keywords");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_UserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_Users_UserId",
                table: "Profiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Profiles",
                table: "Profiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Keywords",
                table: "Keywords");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "usuario");

            migrationBuilder.RenameTable(
                name: "Profiles",
                newName: "perfil");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "notificacao");

            migrationBuilder.RenameTable(
                name: "Keywords",
                newName: "palavra_chave");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "usuario",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "usuario",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "usuario",
                newName: "atualizado_em");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "usuario",
                newName: "senha_hash");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "usuario",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "usuario",
                newName: "criado_em");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "usuario",
                newName: "IX_usuario_email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "perfil",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "perfil",
                newName: "usuario_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "perfil",
                newName: "atualizado_em");

            migrationBuilder.RenameColumn(
                name: "Handle",
                table: "perfil",
                newName: "handle_perfil");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "perfil",
                newName: "criado_em");

            migrationBuilder.RenameIndex(
                name: "IX_Profiles_UserId",
                table: "perfil",
                newName: "IX_perfil_usuario_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "notificacao",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "notificacao",
                newName: "usuario_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "notificacao",
                newName: "atualizado_em");

            migrationBuilder.RenameColumn(
                name: "TweetUrl",
                table: "notificacao",
                newName: "url_tweet");

            migrationBuilder.RenameColumn(
                name: "TweetPostedAt",
                table: "notificacao",
                newName: "postado_em");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "notificacao",
                newName: "titulo");

            migrationBuilder.RenameColumn(
                name: "ProfileHandle",
                table: "notificacao",
                newName: "handle_perfil");

            migrationBuilder.RenameColumn(
                name: "IsRead",
                table: "notificacao",
                newName: "foi_lida");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "notificacao",
                newName: "criado_em");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "notificacao",
                newName: "conteudo");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_UserId",
                table: "notificacao",
                newName: "IX_notificacao_usuario_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "palavra_chave",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "palavra_chave",
                newName: "usuario_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "palavra_chave",
                newName: "atualizado_em");

            migrationBuilder.RenameColumn(
                name: "Term",
                table: "palavra_chave",
                newName: "termo");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "palavra_chave",
                newName: "criado_em");

            migrationBuilder.RenameIndex(
                name: "IX_Keywords_UserId",
                table: "palavra_chave",
                newName: "IX_palavra_chave_usuario_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_usuario",
                table: "usuario",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_perfil",
                table: "perfil",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_notificacao",
                table: "notificacao",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_palavra_chave",
                table: "palavra_chave",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_notificacao_usuario_usuario_id",
                table: "notificacao",
                column: "usuario_id",
                principalTable: "usuario",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_palavra_chave_usuario_usuario_id",
                table: "palavra_chave",
                column: "usuario_id",
                principalTable: "usuario",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_perfil_usuario_usuario_id",
                table: "perfil",
                column: "usuario_id",
                principalTable: "usuario",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notificacao_usuario_usuario_id",
                table: "notificacao");

            migrationBuilder.DropForeignKey(
                name: "FK_palavra_chave_usuario_usuario_id",
                table: "palavra_chave");

            migrationBuilder.DropForeignKey(
                name: "FK_perfil_usuario_usuario_id",
                table: "perfil");

            migrationBuilder.DropPrimaryKey(
                name: "PK_usuario",
                table: "usuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_perfil",
                table: "perfil");

            migrationBuilder.DropPrimaryKey(
                name: "PK_palavra_chave",
                table: "palavra_chave");

            migrationBuilder.DropPrimaryKey(
                name: "PK_notificacao",
                table: "notificacao");

            migrationBuilder.RenameTable(
                name: "usuario",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "perfil",
                newName: "Profiles");

            migrationBuilder.RenameTable(
                name: "palavra_chave",
                newName: "Keywords");

            migrationBuilder.RenameTable(
                name: "notificacao",
                newName: "Notifications");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "senha_hash",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Users",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "criado_em",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "atualizado_em",
                table: "Users",
                newName: "UpdatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_usuario_email",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Profiles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "Profiles",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "handle_perfil",
                table: "Profiles",
                newName: "Handle");

            migrationBuilder.RenameColumn(
                name: "criado_em",
                table: "Profiles",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "atualizado_em",
                table: "Profiles",
                newName: "UpdatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_perfil_usuario_id",
                table: "Profiles",
                newName: "IX_Profiles_UserId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Keywords",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "Keywords",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "termo",
                table: "Keywords",
                newName: "Term");

            migrationBuilder.RenameColumn(
                name: "criado_em",
                table: "Keywords",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "atualizado_em",
                table: "Keywords",
                newName: "UpdatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_palavra_chave_usuario_id",
                table: "Keywords",
                newName: "IX_Keywords_UserId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Notifications",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "Notifications",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "url_tweet",
                table: "Notifications",
                newName: "TweetUrl");

            migrationBuilder.RenameColumn(
                name: "titulo",
                table: "Notifications",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "postado_em",
                table: "Notifications",
                newName: "TweetPostedAt");

            migrationBuilder.RenameColumn(
                name: "handle_perfil",
                table: "Notifications",
                newName: "ProfileHandle");

            migrationBuilder.RenameColumn(
                name: "foi_lida",
                table: "Notifications",
                newName: "IsRead");

            migrationBuilder.RenameColumn(
                name: "criado_em",
                table: "Notifications",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "conteudo",
                table: "Notifications",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "atualizado_em",
                table: "Notifications",
                newName: "UpdatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_notificacao_usuario_id",
                table: "Notifications",
                newName: "IX_Notifications_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Profiles",
                table: "Profiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Keywords",
                table: "Keywords",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Keywords_Users_UserId",
                table: "Keywords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_Users_UserId",
                table: "Profiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
