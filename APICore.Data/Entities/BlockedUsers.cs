namespace APICore.Data.Entities
{
    public class BlockedUsers
    {
        public int Id { get; set; }
        public int BlockerUserId { get; set; } // El ID del usuario que realiza el bloqueo.
        public int BlockedUserId { get; set; } // El ID del usuario que se bloquea.
        public DateTime BlockDateTime { get; set; } // Fecha y hora en que se realizó el bloqueo.

        public User BlockerUser { get; set; } // Referencia al usuario que realiza el bloqueo.
        public User BlockedUser { get; set; } // Referencia al usuario que se bloquea.
    }
}