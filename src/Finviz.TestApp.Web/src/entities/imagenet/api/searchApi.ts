import { api } from "@/shared/api/httpClient";
import type { SearchResultItem } from "../model/types";

export interface SearchResponse {
    items: SearchResultItem[];
    total: number;
}

export async function searchNodes(query: string): Promise<SearchResponse> {
    if (!query.trim()) {
        return { items: [], total: 0 };
    }

    const { data } = await api.get<SearchResponse>("/api/imagenet/search", {
        params: { q: query, take: 50, skip: 0 }
    });

    return data;
}
