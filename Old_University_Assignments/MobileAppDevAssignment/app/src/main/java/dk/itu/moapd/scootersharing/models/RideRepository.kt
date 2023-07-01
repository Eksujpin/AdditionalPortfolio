package dk.itu.moapd.scootersharing.models
import android.app.Application
import androidx.lifecycle.LiveData


class RideRepository(application: Application) {
    private val rideDAO: RideDAO
    private val allRides: LiveData<List<Ride>>

    // Initializes the class attributes.
    init {
        val db = AppDatabase.getDatabase(application)
        rideDAO = db.rideDAO()
        allRides = rideDAO.getOrderedRides()
    }

    suspend fun insert(ride: Ride) {
        rideDAO.insert(ride)
    }

    suspend fun update(ride: Ride) {
        rideDAO.update(ride)
    }

    suspend fun delete(ride: Ride) {
        rideDAO.delete(ride)
    }


    fun getAllRides () = allRides

    fun getAllRideByUser(userID: String) = rideDAO.getByUser(userID)

}