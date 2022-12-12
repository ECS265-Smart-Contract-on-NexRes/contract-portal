import { DeleteBinary } from "./components/DeleteBinary";
import { Home } from "./components/Home";
import { Upload } from "./components/Upload"
import { Transfer } from "./components/Transfer"

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/upload',
    element: <Upload />
  },
  {
    path: '/transfer',
    element: <Transfer />
  }
];

export default AppRoutes;
