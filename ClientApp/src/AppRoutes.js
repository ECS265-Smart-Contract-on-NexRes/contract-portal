import { DeleteBinary } from "./components/DeleteBinary";
import { Home } from "./components/Home";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/delete',
    element: <DeleteBinary />
  }
];

export default AppRoutes;
