import { DeleteBinary } from "./components/DeleteBinary";
import { Home } from "./components/Home";
import { Upload } from "./components/Upload"

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/upload',
    element: <Upload />
  }
];

export default AppRoutes;
