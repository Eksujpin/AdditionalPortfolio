package dk.itu.moapd.scootersharing.fragments

import android.annotation.SuppressLint
import android.app.AlertDialog
import android.content.DialogInterface
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.ImageView
import android.widget.TextView
import android.widget.Toast
import androidx.fragment.app.Fragment
import androidx.fragment.app.activityViewModels
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.google.firebase.ktx.Firebase
import com.google.firebase.storage.ktx.storage
import dk.itu.moapd.scootersharing.R
import dk.itu.moapd.scootersharing.activities.ScooterSharingActivity
import dk.itu.moapd.scootersharing.databinding.FragmentRidesListBinding
import dk.itu.moapd.scootersharing.models.RideVM
import dk.itu.moapd.scootersharing.models.Scooter
import dk.itu.moapd.scootersharing.models.ScooterVM
import org.w3c.dom.Text


class ScooterListFragment : Fragment() {
    // GUI variables
    private var _binding: FragmentRidesListBinding? = null
    private val binding get() = _binding!!
    private val scooterVM: ScooterVM by activityViewModels()
    private lateinit var listView: RecyclerView
    private lateinit var adapter: CardAdapter
    companion object {
        private val TAG = CardAdapter::class.qualifiedName
    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        _binding = FragmentRidesListBinding.inflate(inflater, container, false)

        //List
        listView = binding.ridesList
        listView.layoutManager = LinearLayoutManager(binding.root.context)

        scooterVM.getAll().observe(viewLifecycleOwner, { data ->
            adapter = CardAdapter(data.toCollection(ArrayList()))
            listView.adapter = adapter
        })


        return binding.root
    }

    inner class CardAdapter(private val data: ArrayList<Scooter>) :
        RecyclerView.Adapter<CardViewHolder>() {

        override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): CardViewHolder {
            // Create a new view, which defines the UI of the list item
            val view = LayoutInflater.from(parent.context)
                .inflate(R.layout.list_scooters, parent, false)
            return CardViewHolder(view)
        }
        override fun getItemCount() = data.size

        @SuppressLint("SetTextI18n")
        override fun onBindViewHolder(holder: CardViewHolder, position: Int) {
            val scooter = data[position]
            val storage = Firebase.storage("gs://scooter-sharing-42536.appspot.com/")
            val imageRef = storage.reference.child(scooter.scooterPicture)
            imageRef.downloadUrl.addOnSuccessListener { url ->
                Glide.with(holder.itemView.context)
                    .load(url)
                    .override(400,400)
                    .fitCenter()
                    .into(holder.image)
            }
            Log.i(TAG, "Populate an item at position: $position")
            // Bind the view holder with the selected `String` data.
            holder.apply {
                name.text = scooter.scooterName
                where.text = scooter.scooterLocation
                scooterID.text = "ID: "+scooter.id.toString()
                timestamp.text = scooter.prettyDate()
                removeRide.setOnClickListener{
                    val dialogClickListener = DialogInterface.OnClickListener { _, which ->
                        when (which) {
                            DialogInterface.BUTTON_POSITIVE -> {
                                if (ScooterSharingActivity.lastReserved == scooter.id){
                                    Toast.makeText(binding.root.context, "Can't delete a reserved scooter", Toast.LENGTH_SHORT).show()
                                }else {
                                    ScooterSharingActivity.lastReserved = null
                                    scooterVM.delete(scooter)
                                }
                            }
                            DialogInterface.BUTTON_NEGATIVE -> {

                            }
                        }
                    }
                    val builder: AlertDialog.Builder = AlertDialog.Builder(itemView.context)
                    builder.setMessage(
                        "Are you sure you want to delete: \n$scooter")
                        .setPositiveButton("Yes", dialogClickListener)
                        .setNegativeButton("No", dialogClickListener)
                        .show()
                }
                editRide.setOnClickListener {
                    if (ScooterSharingActivity.lastReserved == scooter.id){
                        Toast.makeText(binding.root.context, "Can't edit a reserved scooter", Toast.LENGTH_SHORT).show()
                    }else{
                        ScooterSharingActivity.editThisScooter = position
                        activity!!.supportFragmentManager.beginTransaction().replace(R.id.fragment_container, EditScooterFragment()).commit()
                    }
                }
                reserveRide.setOnClickListener {
                    if (ScooterSharingActivity.lastReserved == null ){
                        ScooterSharingActivity.lastReserved = scooter.id
                        activity!!.supportFragmentManager.beginTransaction().replace(R.id.fragment_container, RideFragment()).commit()
                    }else Toast.makeText(binding.root.context, "you have already selected a scooter", Toast.LENGTH_SHORT).show()
                }
            }
        }
    }

    inner class CardViewHolder(view: View) : RecyclerView.ViewHolder(view) {
        val name: TextView = view.findViewById(R.id.cardTitle)
        val image: ImageView = view.findViewById(R.id.cardImage)
        val scooterID: TextView = view.findViewById(R.id.cardSubText)
        val where: TextView = view.findViewById(R.id.cardSubText2)
        val timestamp: TextView = view.findViewById(R.id.cardSubText3)
        val editRide: Button = view.findViewById(R.id.editRide)
        val removeRide: Button = view.findViewById(R.id.removeRide)
        val reserveRide: Button = view.findViewById(R.id.reserveRide)
    }

}