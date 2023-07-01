package dk.itu.moapd.scootersharing.models

import android.content.Context
import androidx.room.Database
import androidx.room.Room
import androidx.room.RoomDatabase

@Database(entities = [Scooter::class, Ride::class], version = 1, exportSchema = false)
abstract class AppDatabase : RoomDatabase() {
    abstract fun scooterDAO(): ScooterDAO
    abstract fun rideDAO(): RideDAO
    companion object {
        @Volatile
        private var INSTANCE : AppDatabase? = null
        // This singleton prevents multiple instance of database opening at the same time.
        fun getDatabase(context: Context): AppDatabase {
            return INSTANCE ?: synchronized(this) {
                val instance = Room.databaseBuilder(
                    context.applicationContext,
                    AppDatabase::class.java,
                    "database_name"
                ).build()
                INSTANCE = instance
                // return instance
                instance
            }
        }
    }
}
        /*
        //NaVi Scooters
        rides.add(Scooter("s1mple","DR Metro",randomDate()))
        rides.add(Scooter("b1t","ITU",randomDate()))
        rides.add(Scooter("electroNic","Fields",randomDate()))
        rides.add(Scooter("Perfecto","Fisketorvet",randomDate()))
        rides.add(Scooter("Boombl4","Fælledparken",randomDate()))
        //Gambit Scooters
        rides.add(Scooter("HObbit","DTU",randomDate()))
        rides.add(Scooter("interz","KU",randomDate()))
        rides.add(Scooter("Ax1Le","Ismageriet",randomDate()))
        rides.add(Scooter("sh1ro","Tietgenkollegiet",randomDate()))
        rides.add(Scooter("nafany","Grønjordskollegiet",randomDate()))
        //NIP scooters might be made at some point
        */
