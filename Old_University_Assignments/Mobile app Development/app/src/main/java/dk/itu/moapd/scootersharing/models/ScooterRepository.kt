package dk.itu.moapd.scootersharing.models
import android.app.Application
import androidx.lifecycle.LiveData


class ScooterRepository(application: Application) {
    private val scooterDAO: ScooterDAO
    private val allScooters: LiveData<List<Scooter>>

    // Initializes the class attributes.
    init {
        val db = AppDatabase.getDatabase(application)
        scooterDAO = db.scooterDAO()
        allScooters = scooterDAO.getOrdedScooters()
    }

    suspend fun insert(scooter: Scooter) {
        scooterDAO.insert(scooter)
    }

    suspend fun update(scooter: Scooter) {
        scooterDAO.update(scooter)
    }

    suspend fun delete(scooter: Scooter) {
        scooterDAO.delete(scooter)
    }


    fun getAllScooters () = allScooters

    fun getByName(name: String) = scooterDAO.getByName(name)

    fun getById(id: Int) = scooterDAO.getById(id)

}