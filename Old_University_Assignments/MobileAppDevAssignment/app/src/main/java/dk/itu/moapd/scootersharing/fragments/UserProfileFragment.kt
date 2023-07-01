package dk.itu.moapd.scootersharing.fragments

import android.annotation.SuppressLint
import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.fragment.app.Fragment
import androidx.fragment.app.activityViewModels
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.auth.FirebaseUser
import dk.itu.moapd.scootersharing.R
import dk.itu.moapd.scootersharing.activities.LoginActivity
import dk.itu.moapd.scootersharing.databinding.FragmentUserProfileBinding
import dk.itu.moapd.scootersharing.models.Ride
import dk.itu.moapd.scootersharing.models.RideVM

class UserProfileFragment : Fragment() {
    private lateinit var auth: FirebaseAuth
    private lateinit var user: FirebaseUser
    private lateinit var listView: RecyclerView
    private lateinit var adapter: CustomAdapter
    private var _binding: FragmentUserProfileBinding? = null
    private val binding get() = _binding!!
    private val rideVM: RideVM by activityViewModels()

    companion object {
        private val TAG = CustomAdapter::class.qualifiedName
    }

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View {
        _binding = FragmentUserProfileBinding.inflate(inflater, container, false)

        // get user
        auth = FirebaseAuth.getInstance()
        user = auth.currentUser!!
        binding.userNameText.text = user.displayName
        binding.userMailText.text = user.email

        listView = binding.ridelist
        listView.layoutManager = LinearLayoutManager(binding.root.context)

        rideVM.getAllRideByUser(user.uid).observe(viewLifecycleOwner, { data ->
            adapter = CustomAdapter(data!!.toCollection(ArrayList()))
            listView.adapter = adapter
        })

        binding.editUser.setOnClickListener {
            activity!!.supportFragmentManager.beginTransaction().replace(R.id.fragment_container, EditUserFragment()).commit()
        }

        binding.signOutBtn.setOnClickListener{
            auth.signOut()
            val intent = Intent(activity, LoginActivity::class.java)
            startActivity(intent)
        }

        return binding.root
    }


    inner class CustomAdapter(private val data: ArrayList<Ride>) :
        RecyclerView.Adapter<ViewHolder>() {
        override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
            // Create a new view, which defines the UI of the list item
            val view = LayoutInflater.from(parent.context)
                .inflate(R.layout.list_user_ride_history, parent, false)
            return ViewHolder(view)
        }
        override fun getItemCount() = data.size

        @SuppressLint("SetTextI18n")
        override fun onBindViewHolder(holder: ViewHolder, position: Int) {
            val ride = data.reversed()[position]
            Log.i(TAG, "Populate an item at position: $position")
            // Bind the view holder with the selected `String` data.
            holder.apply {
                date.text = "Date: \n"+ride.prettyDateEnd()
                duration.text = "Duration: \n"+ride.rideTime()
                price.text = "Cost: \n"+ride.ridePrice.toString()+" Kr"
            }
        }
    }

    inner class ViewHolder(view: View) : RecyclerView.ViewHolder(view) {
        val date: TextView = view.findViewById(R.id.ride_date)
        val duration: TextView = view.findViewById(R.id.ride_time)
        val price: TextView = view.findViewById(R.id.ride_price)

    }



}

