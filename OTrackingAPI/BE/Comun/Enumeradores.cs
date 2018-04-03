namespace BE.Comun
{
    public class Enumeradores
    {
        public enum GrupoParametros
        {
            Roles = 100,
            TipoDocumentos = 101,
            Generos = 102,
            GradoDeInstruccion = 103,
            EstadoCivil = 104,
            SiNo = 105,
            TipoExamen = 106,
            Aptitud = 107,
            FactorRH = 108,
            GrupoSanguineo = 109,
            TipoEmpresas = 110,
            Zonas = 111,
            Correo = 112,
            TipoValorComponente = 113,
            TipoTiempo = 114,
            TipoSeguimiento = 115,
            StatusSeguimiento = 116
        }


        public enum EsEliminado
        {
            No = 0,
            Si = 1
        }

        public enum RecibirCorreoDeArea
        {
            No = 0,
            Si = 1
        }

        public enum RecordStatus
        {
            Grabado = 1,
            Agregar = 2,
            Editar = 3,
            Eliminar = 4
        }

        public enum Correo
        {
            HostSMTP = 1,
            CorreoSistema = 2,
            ClaveCorreo = 3,
            BodyEnvioEMO = 4,
            BodyEnvioRegistroUsuario = 5
        }

        public enum TipoTiempo
        {
            Dia = 1,
            Semana = 2,
            Mes = 3,
            Año = 4
        }

        public enum TipoSeguimiento
        {
            Consulta = 1,
            Control = 2
        }

        public enum StatusSeguimiento
        {
            PorConfirmar = 1,
            Confirmado = 2,
            Atendido = 3,
            Vencido = 4
        }
    }
}
