using BE.Comun;
using BE.Acceso;
using BE.Persona;
using BE.Administracion;
using BE.Seguimiento;
using System.Data.Entity;

namespace DAL
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("name=BDOTracking") {}
        public DbSet<Persona> Personas  { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Colaborador> Colaboradores { get; set; }
        public DbSet<Diagnostico> Diagnosticos { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Componente> Componentes { get; set; }
        public DbSet<ComponenteDiagnostico> ComponenteDiagnosticos { get; set; }
        public DbSet<ServicioComponente> ServicioComponentes { get; set; }
        public DbSet<ComponenteCampo> ComponenteCampos { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Perfil> Perfiles { get; set; }
        public DbSet<Parametro> Parametros { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<PlanDeVida> PlanesDeVida { get; set; }
        public DbSet<tblCIE10> CIE10 { get; set; }
        public DbSet<DiagnosticoPlanDeVida> DiagnosticoPlanesDeVida { get; set; }
        public DbSet<CitaPlanDeVida> CitaPlanesDeVida { get; set; }
        public DbSet<Seguimiento> Seguimientos { get; set; }
        public DbSet<FechaSeguimiento> FechasSeguimiento { get; set; }
    }
}
