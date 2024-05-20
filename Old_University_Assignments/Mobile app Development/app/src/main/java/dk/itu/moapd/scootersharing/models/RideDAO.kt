package dk.itu.moapd.scootersharing.models

import androidx.lifecycle.LiveData
import androidx.room.*
@Dao
interface RideDAO {

    @Insert
    suspend fun insert(ride: Ride)

    @Update
    suspend fun update (ride: Ride)

    @Delete
    suspend fun delete (ride: Ride)

    @Query("DELETE FROM ride_table")
    suspend fun deleteAll()

    @Query("SELECT * FROM ride_table ORDER BY ID ASC") //ORDER BY ID ASC
    fun getOrderedRides(): LiveData<List<Ride>>

    @Query("SELECT * FROM ride_table WHERE rideUser LIKE :userID")
    fun getByUser(userID: String): LiveData<List<Ride>?>

}