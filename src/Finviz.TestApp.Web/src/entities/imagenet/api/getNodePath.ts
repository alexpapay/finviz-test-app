import { api } from "@/shared/api/httpClient";
import type { ImageNetNode } from "../model/types";

const PATH_ENDPOINT = "/api/imagenet/tree/path";

export const getNodePath = async (id: number): Promise<ImageNetNode[]> => {
    try {
        const { data } = await api.get<ImageNetNode[]>(`${PATH_ENDPOINT}/${id}`);
        return data;
    } catch (error) {
        return [];
    }
};
